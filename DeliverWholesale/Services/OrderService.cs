using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(int userId, OrderCreateDto dto)
        {
            var config = await _context.Configs.FirstAsync();

            var order = new Order { UserId = userId };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;

            var productIds = dto.Items.Select(i => i.ProduitId).ToList();
            var products = await _context.Produits
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync();

            foreach (var item in dto.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProduitId);
                if (product == null)
                    throw new Exception("Produit invalide");

                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProduitId = product.Id,
                    Quantite = item.Quantite,
                    PrixUnitaire = product.Prix
                };

                total += detail.SousTotal;
                _context.OrderDetails.Add(detail);
                await _context.SaveChangesAsync();
                var lot = await _context.StockLots
                    .Include(l => l.AchatLot)
                    .Where(l => l.AchatLot.ProduitId == product.Id && l.QuantiteRestante > 0)
                    .OrderBy(l => l.DateReception)
                    .FirstOrDefaultAsync();

                if (lot != null)
                {
                    _context.Transactions.Add(new Transaction
                    {
                        StockLotId = lot.Id,
                        OrderDetailId = detail.Id,
                        Type = TypeMouvement.Sortie,
                        Quantite = item.Quantite,
                        DateMouvement = DateTime.UtcNow
                    });
                }
            }

            if (total < config.MontantMinimumCommande)
                throw new Exception("Montant minimum non atteint");

            order.TotalProduits = total;
            order.FraisLivraison = config.FraisLivraison;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task RevertStock(Order order)
        {
            var detailsIds = order.OrderDetails.Select(d => d.Id).ToList();

            var transactions = await _context.Transactions
                .Where(t => t.OrderDetailId.HasValue
                         && detailsIds.Contains(t.OrderDetailId.Value)
                         && t.Type == TypeMouvement.Sortie)
                .ToListAsync();

            foreach (var t in transactions)
            {
                _context.Transactions.Add(new Transaction
                {
                    StockLotId = t.StockLotId,
                    Type = TypeMouvement.Ajustement,
                    Quantite = t.Quantite,
                    DateMouvement = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}