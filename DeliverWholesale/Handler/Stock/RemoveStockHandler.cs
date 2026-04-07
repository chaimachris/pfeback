using DeliverWholesale.Data;
using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.Stock
{
    public class RemoveStockCommand : IRequest<bool>
    {
        public int StockLotId { get; set; }
        public int Quantite { get; set; }

        public RemoveStockCommand(int stockLotId, int quantite)
        {
            StockLotId = stockLotId;
            Quantite = quantite;
        }
    }

    public class RemoveStockHandler : IRequestHandler<RemoveStockCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public RemoveStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveStockCommand request, CancellationToken cancellationToken)
        {
            var stock = await _context.StockLots.FindAsync(request.StockLotId);

            if (stock == null)
                return false;

            if (stock.QuantiteAchetee < request.Quantite)
                return false;

            stock.QuantiteAchetee -= request.Quantite;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}