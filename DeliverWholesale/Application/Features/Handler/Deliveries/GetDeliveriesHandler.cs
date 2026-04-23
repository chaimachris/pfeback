using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record GetDeliveriesQuery() : IRequest<List<Domain.Entities.Delivery>>;

    public class GetDeliveriesHandler : IRequestHandler<GetDeliveriesQuery, List<Domain.Entities.Delivery>>
    {
        private readonly ApplicationDbContext _context;

        public GetDeliveriesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.Delivery>> Handle(
            GetDeliveriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .ToListAsync(cancellationToken);
        }
    }
}