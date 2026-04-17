using DeliverWholesale.Data;
using DeliverWholesale.Handler.Stock;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Products
{
    public class GetProductsQuery : IRequest<List<Produit>>
    {
    }

    public class GetProductStockHandler : IRequestHandler<GetProductStockQuery, List<StockLot>>
    {
        private readonly ApplicationDbContext _context;

        public GetProductStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockLot>> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
        {
            return await _context.StockLots
                .Include(s => s.AchatLot) 
                .Where(s => s.AchatLot.ProduitId == request.ProduitId)
                .ToListAsync(cancellationToken);
        }
    }
}