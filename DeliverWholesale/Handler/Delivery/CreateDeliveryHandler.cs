using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Delivery
{
    public record CreateDeliveryCommand(CreateDeliveryDto Dto) : IRequest<Models.Delivery>;

    public class CreateDeliveryHandler : IRequestHandler<CreateDeliveryCommand, Models.Delivery>
    {
        private readonly ApplicationDbContext _context;

        public CreateDeliveryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Models.Delivery> Handle(CreateDeliveryCommand request, CancellationToken cancellationToken)
        {
            var orderExists = await _context.Orders
        .AnyAsync(o => o.Id == request.Dto.OrderId, cancellationToken);

            if (!orderExists)
            {
                throw new Exception($"Order with Id {request.Dto.OrderId} does not exist.");
            }

            var delivery = new Models.Delivery
            {
                OrderId = request.Dto.OrderId,
                AdresseLivraison = request.Dto.AdresseLivraison,
                DateLivraisonPrevue = request.Dto.DateLivraisonPrevue,
                Statut = DeliveryStatus.EnAttente
            };

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync(cancellationToken);

            return delivery;
        }
    }
}