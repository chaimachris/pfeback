using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class GetProductsQuery : IRequest<List<Produit>> { }


    public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<Produit>>
    {
        private readonly ApplicationDbContext _context;

        public GetProductsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Produit>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Produits
                .Include(p => p.Categorie)
                .Include(p => p.PrixVentes)
                .Include(p => p.StockLots)
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }
    }

}