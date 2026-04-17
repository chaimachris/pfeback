using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Orders
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateOrderHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderDto.Items == null || !request.OrderDto.Items.Any())
                throw new Exception("Aucun produit dans la commande.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = 1, 
                    DateCommande = DateTime.UtcNow,
                    FraisLivraison = 10,
                    TotalProduits = 0,
                    OrderDetails = new List<OrderDetail>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                decimal total = 0;

                foreach (var item in request.OrderDto.Items)
                {
                    if (item.Quantite <= 0)
                        throw new Exception($"Quantité invalide pour produit {item.ProduitId}");

                    var produit = await _context.Produits
                        .FirstOrDefaultAsync(p => p.Id == item.ProduitId);

                    if (produit == null)
                        throw new Exception($"Produit {item.ProduitId} introuvable");

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProduitId = item.ProduitId,
                        Quantite = item.Quantite,
                        PrixUnitaire = produit.Prix
                    };

                    _context.OrderDetails.Add(detail);
                    await _context.SaveChangesAsync();

                    total += detail.Quantite * detail.PrixUnitaire;

                    // FIFO STOCK 
                    int reste = item.Quantite;

                    var stockLots = await _context.StockLots
                        .Include(s => s.AchatLot)
                        .Where(s => s.AchatLot.ProduitId == item.ProduitId && s.QuantiteRestante > 0)
                        .OrderBy(s => s.DateReception)
                        .ToListAsync(cancellationToken);

                    foreach (var lot in stockLots)
                    {
                        if (reste == 0)
                            break;

                        int preleve = Math.Min(lot.QuantiteRestante, reste);

                        if (preleve <= 0) continue;

                        lot.QuantiteRestante -= preleve;

                        _context.LotCommandes.Add(new LotCommande
                        {
                            StockLotId = lot.Id,
                            OrderDetailId = detail.Id,
                            QuantitePrelevee = preleve
                        });

                        _context.Transactions.Add(new Transaction
                        {
                            StockLotId = lot.Id,
                            OrderDetailId = detail.Id,
                            Type = TypeMouvement.Sortie,
                            Quantite = preleve,
                            DateMouvement = DateTime.UtcNow
                        });

                        reste -= preleve;
                    }

                    if (reste > 0)
                        throw new Exception($"Stock insuffisant pour produit {item.ProduitId}");
                }

                order.TotalProduits = total;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return order.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}