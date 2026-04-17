using DeliverWholesale.Data;
using DeliverWholesale.Handler.AchatLot;
using DeliverWholesale.Models;
using MediatR;
using ModelsAchatLot = DeliverWholesale.Models.AchatLot;

namespace DeliverWholesale.Handler.AchatLots
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
            var achat = new ModelsAchatLot
            {
                ProduitId = request.ProduitId,
                QuantiteAchetee = request.QuantiteAchetee,
                PrixUnitaire = request.PrixUnitaire,
                Fournisseur = request.Fournisseur,
                NumeroLot = request.NumeroLot,
                DateAchat = DateTime.UtcNow
            };

            _context.AchatLots.Add(achat);
            await _context.SaveChangesAsync(cancellationToken);

            for (int i = 0; i < request.QuantiteAchetee; i++)
            {
                _context.StockLots.Add(new StockLot
                {
                    AchatLotId = achat.Id,
                    QuantiteRestante = 1,
                    DateReception = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return achat.Id;
        }
    }
}