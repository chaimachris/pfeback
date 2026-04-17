using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.Orders
{
    public class GetAllOrdersQuery : IRequest<List<Order>>
    {
    }
}