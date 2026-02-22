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

            foreach (var item in dto.Items)
            {
                var product = await _context.Produits.FindAsync(item.ProduitId);
                if (product == null || !product.IsActive)
                    throw new Exception("Produit invalide");

                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProduitId = product.Id,
                    Quantite = item.Quantite,
                    PrixUnitaire = product.PrixVente
                };

                total += detail.SousTotal;
                _context.OrderDetails.Add(detail);
                await _context.SaveChangesAsync();

                var lot = await _context.StockLots
                    .Where(l => l.ProduitId == product.Id)
                    .OrderBy(l => l.DateAchat)
                    .FirstOrDefaultAsync();

                if (lot != null)
                {
                    _context.Transactions.Add(new Transaction
                    {
                        StockLotId = lot.Id,
                        OrderDetailId = detail.Id,
                        Type = TypeMouvement.Sortie,
                        Quantite = item.Quantite
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
            foreach (var detail in order.OrderDetails)
            {
                var transactions = await _context.Transactions.Where(t => t.OrderDetailId == detail.Id && t.Type == TypeMouvement.Sortie).ToListAsync();
                foreach (var t in transactions)
                {
                    var adjust = new Transaction
                    {
                        StockLotId = t.StockLotId,
                        Type = TypeMouvement.Ajustement,
                        Quantite = t.Quantite, 
                        DateMouvement = DateTime.UtcNow
                    };
                    _context.Transactions.Add(adjust);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}