using DeliverWholesale.Models;
using MediatR;
using System.Collections.Generic;

namespace DeliverWholesale.Application.AchatLots
{
    public class GetAllAchatLotsQuery : IRequest<List<AchatLot>>
    {
    }
}