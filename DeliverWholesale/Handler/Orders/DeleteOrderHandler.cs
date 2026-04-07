using DeliverWholesale.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DeliverWholesale.Handler.Orders
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteOrderCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteOrderHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.Id);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}