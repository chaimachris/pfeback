using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using DeliverWholesale.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        private readonly IInventoryService _inventory;

        public AddStockHandler(ApplicationDbContext context, IInventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        public async Task<int> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            // 1. On récupère l'AchatLot pour savoir à quel Produit ce stock appartient
            var achatLot = await _context.AchatLots
                .FirstOrDefaultAsync(a => a.Id == request.StockDto.AchatLotId, cancellationToken);

            if (achatLot == null)
                throw new Exception("AchatLot introuvable");

          
            // Delegate to inventory service to create StockLot + Transaction for existing AchatLot
            var stockLotId = await _inventory.ReceiveStockForAchatLotAsync(achatLot.Id, request.StockDto.Quantite, request.StockDto.PrixAchatTotal, request.StockDto.Fournisseur);
            return stockLotId;
        }
    }
}