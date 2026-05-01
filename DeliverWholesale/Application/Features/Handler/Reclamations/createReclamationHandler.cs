using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DeliverWholesale.Application.Features.Handler.Reclamations
{
    public class CreateReclamationHandler
        : IRequestHandler<CreateReclamationCommand, Reclamation>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationService _notificationService;

        public CreateReclamationHandler(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            NotificationService notificationService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
        }

        public async Task<Reclamation> Handle(
            CreateReclamationCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("uid")?.Value;

            var reclamation = new Reclamation
            {
                OrderId = request.Dto.OrderId,
                Sujet = request.Dto.Sujet,
                Description = request.Dto.Description,
                UserId = userId
            };

            _context.Reclamations.Add(reclamation);

            await _context.SaveChangesAsync(cancellationToken);

            
            await _notificationService.NotifyAdmins(
                $"Nouvelle réclamation pour la commande #{request.Dto.OrderId}",
                NotificationType.NewReclamation
            );

            return reclamation;
        }
    }
}