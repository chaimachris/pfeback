using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.AchatLots
{
    public class GetAchatLotByIdQuery : IRequest<AchatLot>
    {
        public int Id { get; set; }

        public GetAchatLotByIdQuery(int id)
        {
            Id = id;
        }
    }
}