using MediatR;

namespace DeliverWholesale.Handler.AchatLots
{
    public class DeleteAchatLotCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}