using DeliverWholesale.Infrastructure.Data;
using MediatR;
using DeliverWholesale.Domain.Entities;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class CreateAchatLotHandler : IRequestHandler<CreateAchatLotCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateAchatLotHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateAchatLotCommand request, CancellationToken cancellationToken)
        {
            var produit = _context.Produits.First(x => x.idP == request.ProduitId);

            var achat = new AchatLot
            {
                ProduitId = request.ProduitId,
                QuantiteAchetee = request.QuantiteAchetee,
                PrixUnitaire = request.PrixUnitaire,
                Fournisseur = request.Fournisseur,
                NumeroLot = request.NumeroLot,
                DateAchat = DateTime.UtcNow,
            };

            _context.AchatLots.Add(achat);
            await _context.SaveChangesAsync(cancellationToken);

            var stockLot = new StockLot()
            {
                AchatLotId = achat.Id,
                DateReception = DateTime.Now,
                QuantiteRestante = achat.QuantiteAchetee * produit.NbUnite,
                ProduitId = achat.ProduitId,
                Produit = achat.Produit
            };

            _context.StockLots.Add(stockLot);
            await _context.SaveChangesAsync(cancellationToken);

            _context.Transactions.Add(new Transaction
            {
                DateMouvement = DateTime.Now,
                Quantite = achat.QuantiteAchetee,
                StockLotId = stockLot.Id,
                StockLot = stockLot,
                Type = Domain.Enums.TypeMouvement.Entree
            });

            await _context.SaveChangesAsync(cancellationToken);

            return achat.Id;
        }
    }
}