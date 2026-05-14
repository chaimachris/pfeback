using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class UpdateAchatLotCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int QuantiteAchetee { get; set; }
        public decimal PrixUnitaire { get; set; }
        public string Fournisseur { get; set; }
        public string NumeroLot { get; set; }
    }

    public class UpdateAchatLotHandler : IRequestHandler<UpdateAchatLotCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        public UpdateAchatLotHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateAchatLotCommand request, CancellationToken cancellationToken)
        {
            var achat = await _context.AchatLots
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (achat == null) return false;

            achat.QuantiteAchetee = request.QuantiteAchetee;
            achat.PrixUnitaire = request.PrixUnitaire;
            achat.Fournisseur = request.Fournisseur;
            achat.NumeroLot = request.NumeroLot;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}