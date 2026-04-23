using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record UpdateDeliveryStatusCommand(int Id, UpdateDeliveryStatusDto Dto) : IRequest<bool>;

    public class UpdateDeliveryStatusHandler : IRequestHandler<UpdateDeliveryStatusCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateDeliveryStatusHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDeliveryStatusCommand request, CancellationToken cancellationToken)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (delivery == null)
                return false;

            delivery.Statut = request.Dto.Statut;

           
            if (request.Dto.Statut == DeliveryStatus.Livree)
            {
                delivery.DateLivraisonReelle = DateTime.UtcNow;
            }
            else
            {
                delivery.DateLivraisonReelle = request.Dto.DateLivraisonReelle;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}