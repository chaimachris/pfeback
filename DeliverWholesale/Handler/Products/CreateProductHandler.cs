using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.Products
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
                PrixAchat = request.ProductDto.PrixAchat,
                PrixVente = request.ProductDto.PrixAchat * 1.2m,
                CategorieId = request.ProductDto.CategorieId
            };

            _context.Produits.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }
    }
}