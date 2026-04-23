using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public int Id { get; set; }
    }
}