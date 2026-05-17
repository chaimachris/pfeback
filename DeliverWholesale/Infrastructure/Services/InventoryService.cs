using System;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DeliverWholesale.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<int, List<LotAllocationPart>>> AllocateOrderStockAsync(int orderId, IEnumerable<AllocateItem> items)
        {
            var result = new Dictionary<int, List<LotAllocationPart>>();

            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var produitIds = items.Select(i => i.ProduitId).Distinct().ToList();

            var nowUtc = DateTime.UtcNow;

            // Exclude expired lots; prefer earliest-expiring lots, then FIFO by DateReception.
            var stockLots = await _context.StockLots
                .Where(sl => produitIds.Contains(sl.ProduitId) && sl.QuantiteRestante > 0 && (sl.ExpirationDate == null || sl.ExpirationDate > nowUtc))
                .OrderBy(sl => sl.ExpirationDate ?? DateTime.MaxValue)
                .ThenBy(sl => sl.DateReception)
                .ToListAsync();

            foreach (var item in items)
            {
                var remaining = item.Quantite;
                result[item.ProduitId] = new List<LotAllocationPart>();

                var lotsForProduit = stockLots.Where(sl => sl.ProduitId == item.ProduitId).ToList();

                foreach (var lot in lotsForProduit)
                {
                    if (remaining <= 0) break;
                    var toTake = Math.Min(lot.QuantiteRestante, remaining);
                    if (toTake <= 0) continue;

                    lot.QuantiteRestante -= toTake;

                    var lotCommande = new LotCommande
                    {
                        StockLotId = lot.Id,
                        OrderDetailId = item.OrderDetailId,
                        QuantitePrelevee = toTake
                    };
                    _context.LotCommandes.Add(lotCommande);

                    _context.Transactions.Add(new Transaction
                    {
                        StockLotId = lot.Id,
                        OrderDetailId = item.OrderDetailId,
                        Type = TypeMouvement.Sortie,
                        Quantite = toTake,
                        DateMouvement = DateTime.UtcNow
                    });

                    result[item.ProduitId].Add(new LotAllocationPart(lot.Id, item.OrderDetailId, toTake));

                    remaining -= toTake;
                }

                if (remaining > 0)
                {
                    throw new ApplicationException($"Stock insuffisant for product {item.ProduitId}. Missing {remaining} units.");
                }
            }

            _context.StockLots.UpdateRange(stockLots);
            try
            {
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                throw new ApplicationException("Concurrency conflict during stock allocation. Please retry the operation.", ex);
            }

            return result;
        }

        public async Task<int> ReceivePurchaseStockAsync(int produitId, int quantiteAchetee, decimal prixUnitaire, string? numeroLot = null, int? supplierId = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var resolvedNumeroLot = string.IsNullOrWhiteSpace(numeroLot)
                ? await GenerateNumeroLotAsync()
                : numeroLot;

            var produit = await _context.Produits.FirstOrDefaultAsync(p => p.idP == produitId);

            var achat = new AchatLot
            {
                ProduitId = produitId,
                QuantiteAchetee = quantiteAchetee,
                PrixUnitaire = prixUnitaire,
                Fournisseur = supplierId?.ToString() ?? string.Empty,
                SupplierId = supplierId,
                NumeroLot = resolvedNumeroLot,
                DateAchat = DateTime.UtcNow
            };

            var stockLot = new StockLot
            {
                AchatLot = achat,
                DateReception = DateTime.UtcNow,
                QuantiteRestante = quantiteAchetee * Math.Max(1, produit?.NbUnite ?? 1),
                ProduitId = produitId,
            };

            var transaction = new Transaction
            {
                StockLot = stockLot,
                Type = TypeMouvement.Entree,
                Quantite = stockLot.QuantiteRestante,
                DateMouvement = DateTime.UtcNow
            };

            _context.AchatLots.Add(achat);
            _context.StockLots.Add(stockLot);
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return achat.Id;
        }

        private async Task<string> GenerateNumeroLotAsync()
        {
            while (true)
            {
                var numeroLot = $"LOT-{Guid.NewGuid():N}";
                var exists = await _context.AchatLots.AnyAsync(a => a.NumeroLot == numeroLot);
                if (!exists)
                {
                    return numeroLot;
                }
            }
        }

        public async Task AdjustStockAsync(int stockLotId, int delta, TypeMouvement type, int? orderDetailId = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var lot = await _context.StockLots.FirstOrDefaultAsync(sl => sl.Id == stockLotId);
            if (lot == null) throw new ApplicationException("StockLot not found");

            var final = lot.QuantiteRestante + delta;
            if (final < 0) throw new ApplicationException("Operation would result in negative stock");

            lot.QuantiteRestante = final;

            _context.Transactions.Add(new Transaction
            {
                StockLotId = lot.Id,
                OrderDetailId = orderDetailId,
                Type = type,
                Quantite = Math.Abs(delta),
                DateMouvement = DateTime.UtcNow
            });

            _context.StockLots.Update(lot);
            try
            {
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                throw new ApplicationException("Concurrency conflict while adjusting stock. Please retry the operation.", ex);
            }
        }

        public async Task<int> ReceiveStockForAchatLotAsync(int achatLotId, int quantite, decimal prixAchatTotal, string? fournisseur = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var achat = await _context.AchatLots.FirstOrDefaultAsync(a => a.Id == achatLotId);
            if (achat == null) throw new ApplicationException("AchatLot not found");

            var produit = await _context.Produits.FirstOrDefaultAsync(p => p.idP == achat.ProduitId);

            var stockLot = new StockLot
            {
                AchatLotId = achat.Id,
                DateReception = DateTime.UtcNow,
                QuantiteRestante = quantite * Math.Max(1, produit?.NbUnite ?? 1),
                ProduitId = achat.ProduitId
            };

            var transaction = new Transaction
            {
                StockLot = stockLot,
                Type = TypeMouvement.Entree,
                Quantite = stockLot.QuantiteRestante,
                DateMouvement = DateTime.UtcNow
            };

            _context.StockLots.Add(stockLot);
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return stockLot.Id;
        }

        public async Task<int> MigrateSuppliersFromAchatLotsAsync()
        {
            // Idempotent: create Supplier rows for distinct non-empty Fournisseur values and populate AchatLot.SupplierId
            var fournisseurs = await _context.AchatLots
                .Where(a => !string.IsNullOrEmpty(a.Fournisseur))
                .Select(a => a.Fournisseur)
                .Distinct()
                .ToListAsync();

            int created = 0;

            foreach (var f in fournisseurs)
            {
                var existing = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == f);
                if (existing == null)
                {
                    var sup = new Domain.Entities.Supplier { Name = f };
                    _context.Suppliers.Add(sup);
                    await _context.SaveChangesAsync();
                    existing = sup;
                    created++;
                }

                // Link AchatLots that have this Fournisseur and no SupplierId
                var achatLots = await _context.AchatLots.Where(a => a.Fournisseur == f && a.SupplierId == null).ToListAsync();
                foreach (var a in achatLots)
                {
                    a.SupplierId = existing.Id;
                }
                if (achatLots.Any())
                {
                    _context.AchatLots.UpdateRange(achatLots);
                    await _context.SaveChangesAsync();
                }
            }

            return created;
        }

        public async Task RevertOrderStockAsync(int orderId)
        {
            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            await RevertOrderStockTransactionalAsync(orderId);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public async Task RevertOrderStockTransactionalAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new ApplicationException("Order not found");

            var detailIds = order.OrderDetails.Select(d => d.Id).ToList();

            var transactions = await _context.Transactions
                .Where(t => t.OrderDetailId.HasValue && detailIds.Contains(t.OrderDetailId.Value) && t.Type == TypeMouvement.Sortie)
                .ToListAsync();

            foreach (var t in transactions)
            {
                var lot = await _context.StockLots.FirstOrDefaultAsync(sl => sl.Id == t.StockLotId);
                if (lot == null) continue;

                lot.QuantiteRestante += t.Quantite;

                _context.Transactions.Add(new Transaction
                {
                    StockLotId = t.StockLotId,
                    Type = TypeMouvement.Ajustement,
                    Quantite = t.Quantite,
                    DateMouvement = DateTime.UtcNow
                });

                _context.StockLots.Update(lot);
            }
        }

        public async Task<(bool Sufficient, Dictionary<int,int> Shortages)> ValidateAvailabilityAsync(IEnumerable<(int ProduitId, int Quantite)> items)
        {
            var shortages = new Dictionary<int, int>();
            var produitIds = items.Select(i => i.ProduitId).Distinct().ToList();

            var now = DateTime.UtcNow;

            // Consider only non-expired lots for availability
            var stockSums = await _context.StockLots
                .Where(sl => produitIds.Contains(sl.ProduitId) && (sl.ExpirationDate == null || sl.ExpirationDate > now))
                .GroupBy(sl => sl.ProduitId)
                .Select(g => new { ProduitId = g.Key, Total = g.Sum(x => x.QuantiteRestante) })
                .ToListAsync();

            foreach (var item in items)
            {
                var sum = stockSums.FirstOrDefault(s => s.ProduitId == item.ProduitId)?.Total ?? 0;
                if (sum < item.Quantite)
                    shortages[item.ProduitId] = item.Quantite - sum;
            }

            return (shortages.Count == 0, shortages);
        }
    }
}
