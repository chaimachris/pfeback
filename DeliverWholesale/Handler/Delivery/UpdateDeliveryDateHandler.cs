using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Delivery
{
    public class UpdateDeliveryDateHandler : IRequestHandler<UpdateDeliveryDateCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateDeliveryDateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDeliveryDateCommand request, CancellationToken cancellationToken)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (delivery == null)
                return false;

            delivery.DateLivraisonPrevue = request.NewDate;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}