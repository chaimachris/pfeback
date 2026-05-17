using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventory;

        public DeleteOrderHandler(ApplicationDbContext context, IInventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null)
                return false;

            // If order already cancelled, nothing to do
            if (order.Statut == Domain.Entities.StatutOrder.Annulee)
                return false;

            // Begin transaction so revert + status change are atomic
            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Use InventoryService to revert stock for this order deterministically
            await _inventory.RevertOrderStockTransactionalAsync(order.Id);

            // Mark order as cancelled instead of deleting history
            order.Statut = Domain.Entities.StatutOrder.Annulee;
            _context.Orders.Update(order);

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return true;
        }
    }
}