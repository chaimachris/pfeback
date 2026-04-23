using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class GetAllOrdersQuery : IRequest<List<Order>>
    {
    }
}