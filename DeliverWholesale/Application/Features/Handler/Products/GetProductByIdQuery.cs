using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class GetProductByIdQuery : IRequest<Produit?>
    {
        public int Id { get; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Produit?>
    {
        private readonly ApplicationDbContext _context;

        public GetProductByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Produit?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Produits
                .Include(p => p.Categorie)
                .Include(p => p.PrixVentes)
                .Include(p => p.StockLots)
                .FirstOrDefaultAsync(p => p.idP == request.Id && p.IsActive, cancellationToken);
        }
    }
}
