using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}