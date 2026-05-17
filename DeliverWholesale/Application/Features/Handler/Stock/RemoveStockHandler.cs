using DeliverWholesale.Infrastructure.Services;
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
        private readonly IInventoryService _inventory;

        public RemoveStockHandler(ApplicationDbContext context, IInventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        public async Task<bool> Handle(RemoveStockCommand request, CancellationToken cancellationToken)
        {
            var stock = await _context.StockLots
                .AsTracking()
                .FirstOrDefaultAsync(s => s.Id == request.StockLotId, cancellationToken);

            if (stock == null)
                return false;

            if (stock.QuantiteRestante < request.Quantite)
                return false;

            // Use InventoryService to create a ledger entry and update stock atomically
            await _inventory.AdjustStockAsync(request.StockLotId, -request.Quantite, Domain.Enums.TypeMouvement.Sortie, null);

            return true;
        }
    }
}
