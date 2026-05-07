using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class CreateProductCommand : IRequest<int>
    {
        public ProductCreateDto ProductDto { get; set; }

        public CreateProductCommand(ProductCreateDto productDto)
        {
            ProductDto = productDto;
        }
    }

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateProductHandler(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            string? imagePath = null;

            if (request.ProductDto.Image != null)
            {
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() +
                               Path.GetExtension(request.ProductDto.Image.FileName);

                var fullPath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);

                await request.ProductDto.Image.CopyToAsync(stream);

                imagePath = "/uploads/" + fileName;
            }

            var product = new Produit
            {
                Nom = request.ProductDto.Nom,
                Description = request.ProductDto.Description,
                PrixAchat = request.ProductDto.Prix,
                CategorieId = request.ProductDto.CategorieId,
                ImageUrl = imagePath,
                IsActive = true
            };

            _context.Produits.Add(product);

            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}