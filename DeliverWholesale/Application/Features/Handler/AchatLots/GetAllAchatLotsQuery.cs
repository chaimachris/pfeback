using DeliverWholesale.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class GetAllAchatLotsQuery : IRequest<List<AchatLot>>
    {
    }
}