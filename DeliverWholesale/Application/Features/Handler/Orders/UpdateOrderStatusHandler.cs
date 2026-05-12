using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateOrderStatusCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.Id);

            if (order == null)
                return false;

            // conversion safe string → enum
            if (!Enum.TryParse<StatutOrder>(request.Statut, true, out var statut))
                throw new Exception("Statut invalide");

            order.Statut = statut;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}