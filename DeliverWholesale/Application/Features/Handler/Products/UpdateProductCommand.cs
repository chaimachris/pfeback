using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{
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
        private readonly IWebHostEnvironment _environment;

        public UpdateProductHandler(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Produits
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return false;

            if (!string.IsNullOrEmpty(request.ProductDto.Nom))
                product.Nom = request.ProductDto.Nom;

            if (!string.IsNullOrEmpty(request.ProductDto.Description))
                product.Description = request.ProductDto.Description;

            if (request.ProductDto.PrixAchat.HasValue)
                product.PrixAchat = request.ProductDto.PrixAchat.Value;

            if (request.ProductDto.CategorieId.HasValue)
                product.CategorieId = request.ProductDto.CategorieId.Value;

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

                product.ImageUrl = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}