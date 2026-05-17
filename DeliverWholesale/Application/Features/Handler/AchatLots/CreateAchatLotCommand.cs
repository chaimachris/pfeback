using MediatR;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class CreateAchatLotCommand : IRequest<int>
    {
        public int ProduitId { get; set; }
        public int QuantiteAchetee { get; set; }
        public decimal PrixUnitaire { get; set; }
        // Backwards-compatible free-text supplier name. Optional during migration.
        public string Fournisseur { get; set; }
        // New structured supplier FK (optional). Set during migration when possible.
        public int? SupplierId { get; set; }
    }
}