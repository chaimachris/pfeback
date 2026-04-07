using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.Stock
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
                ProduitId = request.StockDto.ProduitId,
                QuantiteAchetee = request.StockDto.Quantite,
                PrixAchatLot = request.StockDto.PrixAchatTotal,
                Fournisseur = request.StockDto.Fournisseur,
                Unite = request.StockDto.Unite,
                DateAchat = DateTime.Now
            };

            _context.StockLots.Add(stockLot);
            await _context.SaveChangesAsync();

            return stockLot.Id;
        }
    }
}