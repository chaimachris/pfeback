using DeliverWholesale.DTOs;
using MediatR;

namespace DeliverWholesale.Handler.Orders
{
    public class CreateOrderCommand : IRequest<int>
    {
        public OrderCreateDto OrderDto { get; set; }

        public CreateOrderCommand(OrderCreateDto dto)
        {
            OrderDto = dto;
        }
    }
}