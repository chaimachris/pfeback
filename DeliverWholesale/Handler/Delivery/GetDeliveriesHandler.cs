using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Delivery
{
    public record GetDeliveriesQuery() : IRequest<List<Models.Delivery>>;

    public class GetDeliveriesHandler : IRequestHandler<GetDeliveriesQuery, List<Models.Delivery>>
    {
        private readonly ApplicationDbContext _context;

        public GetDeliveriesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Delivery>> Handle(
            GetDeliveriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .ToListAsync(cancellationToken);
        }
    }
}