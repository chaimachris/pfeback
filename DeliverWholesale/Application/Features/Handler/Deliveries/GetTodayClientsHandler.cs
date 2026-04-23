using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public class GetTodayClientsHandler : IRequestHandler<GetTodayClientsQuery, object>
    {
        private readonly ApplicationDbContext _context;

        public GetTodayClientsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GetTodayClientsQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Deliveries
                .Include(d => d.Order)
                .Where(d => d.Order.DateCommande.Date == today)
                .Select(d => new
                {
                    DeliveryId = d.Id,
                    OrderId = d.OrderId,
                    Adresse = d.AdresseLivraison,
                    Statut = d.Statut
                })
                .ToListAsync(cancellationToken);
        }
    }
}