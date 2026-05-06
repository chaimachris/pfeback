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
            var stock = await _context.StockLots
                .ToListAsync(cancellationToken);
            var productIds = stock.Select(x => x.ProduitId).Distinct().ToList();
            var products = _context.Produits.Where(x => productIds.Contains(x.Id)).ToList();

            var listStock = new List<StockDetailsDTO>(); 

            foreach (var item in productIds)
            {
                var listStockIds = stock.FindAll(x => x.ProduitId == item).Select(x => x.Id).ToList();
                listStock.Add(new StockDetailsDTO
                {
                    Product = products.First(x => x.Id == item),
                    QuantiteTotalRestante = stock.Where(x => x.ProduitId == item).Sum(x => x.QuantiteRestante),
                    StockLotId = listStockIds,
                    Transations = _context.Transactions.Where(x => listStockIds.Contains(x.StockLotId)).ToList()
                });
            }

            return listStock;
        }
    }
}