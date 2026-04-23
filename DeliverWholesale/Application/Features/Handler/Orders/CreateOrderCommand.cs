using DeliverWholesale.Application.DTOs.DTOs;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
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