using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Stock
{
    public class AddStockCommand : IRequest<int>
    {
        public AddStockLotDto StockDto { get; set; }

        public AddStockCommand(AddStockLotDto dto)
        {
            StockDto = dto;
        }
    }

    public class AddStockHandler : IRequestHandler<AddStockCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public AddStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            var stockLot = new StockLot
            {
                AchatLotId = request.StockDto.AchatLotId, 
                QuantiteRestante = request.StockDto.Quantite,
                DateReception = DateTime.UtcNow
            };

            _context.StockLots.Add(stockLot);
            await _context.SaveChangesAsync(cancellationToken);

            return stockLot.Id;
        }
    }
}