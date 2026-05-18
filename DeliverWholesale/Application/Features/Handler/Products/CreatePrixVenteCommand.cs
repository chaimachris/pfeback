using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class CreatePrixVenteCommand : IRequest<int>
    {
        public PrixVenteCreateDto Dto { get; set; }

        public CreatePrixVenteCommand(PrixVenteCreateDto dto)
        {
            Dto = dto;
        }
    }
    public class CreatePrixVenteHandler : IRequestHandler<CreatePrixVenteCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreatePrixVenteHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreatePrixVenteCommand request, CancellationToken cancellationToken)
        {
            var produit = await _context.Produits.FindAsync(new object[] { request.Dto.idP }, cancellationToken);
            if (produit == null)
                throw new Exception("Produit introuvable");

            if (!produit.prixModifiable)
                throw new InvalidOperationException("Le prix de ce produit ne peut pas être modifié car 'prixModifiable' est désactivé.");

            var prix = new PrixVente
            {
                idP = request.Dto.idP,
                Valeur = request.Dto.Valeur,
                Date = DateTime.UtcNow
            };

            _context.PrixVentes.Add(prix);
            await _context.SaveChangesAsync(cancellationToken);

            return prix.Id;
        }
    }

}