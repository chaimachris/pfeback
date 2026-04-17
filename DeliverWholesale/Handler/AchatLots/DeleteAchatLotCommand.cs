using MediatR;

namespace DeliverWholesale.Handler.AchatLot
{
    public class DeleteAchatLotCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}