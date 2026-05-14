using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteOrderHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null)
                return false;

            // 1. Récupérer les IDs des OrderDetails
            var orderDetailIds = order.OrderDetails.Select(od => od.Id).ToList();

            // 2. Supprimer les Transactions liées aux OrderDetails (Clé étrangère Restrict)
            if (orderDetailIds.Any())
            {
                var transactions = await _context.Transactions
                    .Where(t => t.OrderDetailId.HasValue && orderDetailIds.Contains(t.OrderDetailId.Value))
                    .ToListAsync(cancellationToken);

                _context.Transactions.RemoveRange(transactions);
            }

            // 3. Supprimer les OrderDetails
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // 4. Supprimer la Livraison liée si elle existe
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.OrderId == order.Id, cancellationToken);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
            }

            // 5. Supprimer les Réclamations liées si elles existent
            var reclamations = await _context.Reclamations
                .Where(r => r.OrderId == order.Id)
                .ToListAsync(cancellationToken);
            if (reclamations.Any())
            {
                _context.Reclamations.RemoveRange(reclamations);
            }

            // 6. Enfin, supprimer la commande elle-même
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}