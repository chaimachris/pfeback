using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using DeliverWholesale.Domain.Entities;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class CreateAchatLotHandler : IRequestHandler<CreateAchatLotCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventory;

            public CreateAchatLotHandler(ApplicationDbContext context, IInventoryService inventory)
            {
                _context = context;
                _inventory = inventory;
            }

        public async Task<int> Handle(CreateAchatLotCommand request, CancellationToken cancellationToken)
        {
            // Delegate to InventoryService to create AchatLot, StockLot and Transaction atomically
            var achatId = await _inventory.ReceivePurchaseStockAsync(
                request.ProduitId,
                request.QuantiteAchetee,
                request.PrixUnitaire,
                supplierId: request.SupplierId);
            return achatId;
        }
    }
}