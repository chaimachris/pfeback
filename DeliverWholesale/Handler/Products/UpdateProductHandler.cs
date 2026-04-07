using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Products
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
            var product = await _context.Produits.FindAsync(request.Id);

            if (product == null)
                return false;

            product.Nom = request.ProductDto.Nom;
            product.Description = request.ProductDto.Description;
            product.PrixAchat = request.ProductDto.PrixAchat;
            product.PrixVente = request.ProductDto.PrixAchat * 1.2m;
            product.CategorieId = request.ProductDto.CategorieId;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}