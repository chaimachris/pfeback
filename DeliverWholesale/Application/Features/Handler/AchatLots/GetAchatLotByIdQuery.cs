using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
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