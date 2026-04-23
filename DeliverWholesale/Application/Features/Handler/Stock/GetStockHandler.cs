using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Stock
{
    public class GetStockQuery : IRequest<List<StockLot>>
    {
    }

    public class GetStockHandler : IRequestHandler<GetStockQuery, List<StockLot>>
    {
        private readonly ApplicationDbContext _context;

        public GetStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockLot>> Handle(GetStockQuery request, CancellationToken cancellationToken)
        {
            return await _context.StockLots
                .Include(s => s.AchatLot)
                    .ThenInclude(a => a.Produit)
                .ToListAsync(cancellationToken);
        }
    }
}