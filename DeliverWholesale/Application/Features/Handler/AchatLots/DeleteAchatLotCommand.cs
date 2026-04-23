using MediatR;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class DeleteAchatLotCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}