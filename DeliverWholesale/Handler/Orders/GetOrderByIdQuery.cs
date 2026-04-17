using DeliverWholesale.Models;
using MediatR;

namespace DeliverWholesale.Handler.Orders
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public int Id { get; set; }
    }
}