using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{   // ← ACCOLADE OUVRANTE AJOUTÉE ICI

    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public ProductUpdateDto ProductDto { get; set; }

        public UpdateProductCommand(int id, ProductUpdateDto dto)
        {
            Id = id;
            ProductDto = dto;
        }
    }

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UpdateProductHandler(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Produits
                .FirstOrDefaultAsync(p => p.idP == request.Id, cancellationToken);

            if (product == null)
                return false;

            if (!string.IsNullOrEmpty(request.ProductDto.libelle))
                product.libelle = request.ProductDto.libelle;

            if (!string.IsNullOrEmpty(request.ProductDto.Description))
                product.Description = request.ProductDto.Description;

            if (request.ProductDto.idCategorie.HasValue)
                product.idCategorie = request.ProductDto.idCategorie.Value;

            if (request.ProductDto.seuil.HasValue)
                product.seuil = request.ProductDto.seuil.Value;

            if (request.ProductDto.prixModifiable.HasValue)
                product.prixModifiable = request.ProductDto.prixModifiable.Value;

            // Si nouveau prix renseigné → créer une entrée PrixVente
            if (request.ProductDto.NouveauPrixVente.HasValue)
            {
                var prix = new PrixVente
                {
                    idP = product.idP,
                    Valeur = request.ProductDto.NouveauPrixVente.Value,
                    Date = DateTime.UtcNow
                };
                _context.PrixVentes.Add(prix);
            }

            // Image
            if (request.ProductDto.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProductDto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProductDto.Image.CopyToAsync(stream, cancellationToken);
                }

                product.ImageUrl = $"/images/products/{uniqueFileName}";
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}