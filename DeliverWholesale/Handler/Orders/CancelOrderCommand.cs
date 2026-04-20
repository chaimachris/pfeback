using MediatR;

namespace DeliverWholesale.Handler.Orders
{
    public class CancelOrderCommand : IRequest<string>
    {
        public int OrderId { get; set; }

        public CancelOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}