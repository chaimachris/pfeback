using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Stock
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
            var stock = await _context.StockLots
                .FirstOrDefaultAsync(s => s.Id == request.StockLotId, cancellationToken);

            if (stock == null)
                return false;

            if (stock.QuantiteRestante < request.Quantite)
                return false;

            stock.QuantiteRestante -= request.Quantite;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}