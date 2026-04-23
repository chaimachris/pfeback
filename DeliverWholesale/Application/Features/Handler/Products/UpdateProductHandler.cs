using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public ProductCreateDto ProductDto { get; set; }

        public UpdateProductCommand(int id, ProductCreateDto dto)
        {
            Id = id;
            ProductDto = dto;
        }
    }

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProductHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Produits
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return false;

            product.Nom = request.ProductDto.Nom;
            product.Description = request.ProductDto.Description;
            product.PrixAchat = request.ProductDto.Prix;   
            product.CategorieId = request.ProductDto.CategorieId;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}