using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeliverWholesale.Application.DTOs;

namespace DeliverWholesale.Application.Features.Handler.Stock
{
    public class GetStockQuery : IRequest<List<StockDetailsDTO>>
    {
    }

    public class GetStockHandler : IRequestHandler<GetStockQuery, List<StockDetailsDTO>>
    {
        private readonly ApplicationDbContext _context;

        public GetStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockDetailsDTO>> Handle(GetStockQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var stock = await _context.StockLots
                .ToListAsync(cancellationToken);

            var productIds = stock.Select(x => x.ProduitId).Distinct().ToList();

            var products = _context.Produits
                .Include(x => x.PrixVentes)
                .Where(x => productIds.Contains(x.idP))
                .ToList();

            var listStock = new List<StockDetailsDTO>();

            foreach (var item in productIds)
            {
                var listStockIds = stock.FindAll(x => x.ProduitId == item).Select(x => x.Id).ToList();

                // Only count lots that the allocator can actually use: non-expired and with remaining quantity.
                var allocatable = stock.Where(x =>
                    x.ProduitId == item &&
                    x.QuantiteRestante > 0 &&
                    (x.ExpirationDate == null || x.ExpirationDate > now));

                listStock.Add(new StockDetailsDTO
                {
                    Product = products.First(x => x.idP == item),
                    QuantiteTotalRestante = allocatable.Sum(x => x.QuantiteRestante),
                    StockLotId = listStockIds,
                    Transations = _context.Transactions.Where(x => listStockIds.Contains(x.StockLotId)).ToList()
                });
            }

            return listStock;
        }
    }
}