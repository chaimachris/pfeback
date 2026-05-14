using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Application.Features.Handler.Products;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class CreateProductCommand : IRequest<int>
    {
        public ProductCreateDto ProductDto { get; set; }

        public CreateProductCommand(ProductCreateDto dto)
        {
            ProductDto = dto;
        }
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateProductHandler(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        string? imagePath = null;

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

            imagePath = $"/images/products/{uniqueFileName}";
        }

        var product = new Produit
        {
            libelle = request.ProductDto.libelle,
            Description = request.ProductDto.Description,
            idCategorie = request.ProductDto.idCategorie,
            NbUnite = request.ProductDto.NbUnite,
            seuil = request.ProductDto.seuil,
            prixModifiable = request.ProductDto.prixModifiable,
            ImageUrl = imagePath,
            IsActive = true
        };

        _context.Produits.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Enregistrer le prix initial dans PrixVente
        var prix = new PrixVente
        {
            idP = product.idP,
            Valeur = request.ProductDto.PrixVente,
            Date = DateTime.UtcNow
        };
        _context.PrixVentes.Add(prix);
        await _context.SaveChangesAsync(cancellationToken);

        return product.idP;
    }
}

