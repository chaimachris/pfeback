using DeliverWholesale.Models;
using MediatR;
using System.Collections.Generic;

namespace DeliverWholesale.Handler.AchatLots
{
    public class GetAllAchatLotsQuery : IRequest<List<AchatLot>>
    {
    }
}