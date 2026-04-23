using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, string>
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public CancelOrderHandler(
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId);

                if (order == null)
                    throw new Exception("Commande introuvable");

                if (order.Statut == StatutOrder.Annulee)
                    return "Commande déjà annulée";

                var user = await _context.Users.FindAsync(order.UserId);

                //  RESTORE STOCK
                foreach (var detail in order.OrderDetails)
                {
                    var lotCommandes = await _context.LotCommandes
                        .Where(x => x.OrderDetailId == detail.Id)
                        .ToListAsync();

                    foreach (var lc in lotCommandes)
                    {
                        var stockLot = await _context.StockLots
                            .FirstOrDefaultAsync(s => s.Id == lc.StockLotId);

                        if (stockLot != null)
                        {
                            stockLot.QuantiteRestante += lc.QuantitePrelevee;

                            _context.Transactions.Add(new Transaction
                            {
                                StockLotId = stockLot.Id,
                                OrderDetailId = detail.Id,
                                Type = TypeMouvement.Entree,
                                Quantite = lc.QuantitePrelevee,
                                DateMouvement = DateTime.UtcNow
                            });
                        }
                    }
                }

                //  UPDATE STATUS 
                order.Statut = StatutOrder.Annulee;

                await _context.SaveChangesAsync();

                //  NOTIFICATION
                await _notificationService.NotifyAdmins(
                    $"❌ Commande #{order.Id} annulée par {user?.Prenom} {user?.Nom}",
                    NotificationType.CancelOrder
                );

                await transaction.CommitAsync();

                return "Commande annulée avec succès";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}