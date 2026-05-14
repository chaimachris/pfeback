using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
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

        public AddStockHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            // 1. On récupère l'AchatLot pour savoir à quel Produit ce stock appartient
            var achatLot = await _context.AchatLots
                .FirstOrDefaultAsync(a => a.Id == request.StockDto.AchatLotId, cancellationToken);

            if (achatLot == null)
                throw new Exception("AchatLot introuvable");

          
            var stockLot = new StockLot
            {
                AchatLotId = request.StockDto.AchatLotId,
                ProduitId = achatLot.ProduitId, // ✅ AJOUT TRÈS IMPORTANT
                QuantiteRestante = request.StockDto.Quantite,
                DateReception = DateTime.UtcNow
            };

            _context.StockLots.Add(stockLot);

            // 3. Création de la Transaction pour tracer l'entrée en stock
            _context.Transactions.Add(new Transaction
            {
                DateMouvement = DateTime.UtcNow,
                Quantite = request.StockDto.Quantite,
                StockLot = stockLot,
                Type = Domain.Enums.TypeMouvement.Entree // ✅ AJOUT TRÈS IMPORTANT
            });

            await _context.SaveChangesAsync(cancellationToken);

            return stockLot.Id;
        }
    }
}