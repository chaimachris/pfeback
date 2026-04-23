using MediatR;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class CreateAchatLotCommand : IRequest<int>
    {
        public int ProduitId { get; set; }
        public int QuantiteAchetee { get; set; }
        public decimal PrixUnitaire { get; set; }
        public string Fournisseur { get; set; }
        public string NumeroLot { get; set; }
    }
}