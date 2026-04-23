using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Domain.Entities;
using MediatR;

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

        public CreateProductHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Produit
            {
                Nom = request.ProductDto.Nom,
                Description = request.ProductDto.Description,
                PrixAchat = request.ProductDto.Prix,   
                CategorieId = request.ProductDto.CategorieId,
                SeuilAlerte = request.ProductDto.SeuilAlerte,
                IsActive = true
            };

            _context.Produits.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}