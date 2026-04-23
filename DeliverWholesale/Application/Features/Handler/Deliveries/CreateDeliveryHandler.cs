using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record CreateDeliveryCommand(CreateDeliveryDto Dto) : IRequest<Delivery>;

    public class CreateDeliveryHandler : IRequestHandler<CreateDeliveryCommand, Delivery>
    {
        private readonly ApplicationDbContext _context;

        public CreateDeliveryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Delivery> Handle(CreateDeliveryCommand request, CancellationToken cancellationToken)
        {
            var orderExists = await _context.Orders
                .AnyAsync(o => o.Id == request.Dto.OrderId, cancellationToken);

            if (!orderExists)
                throw new Exception($"Order {request.Dto.OrderId} does not exist.");

            var delivery = new Delivery 
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