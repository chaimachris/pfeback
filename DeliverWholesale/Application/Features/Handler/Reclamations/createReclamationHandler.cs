using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new ApplicationException("Utilisateur non authentifié.");

            var userId = userIdClaim;

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