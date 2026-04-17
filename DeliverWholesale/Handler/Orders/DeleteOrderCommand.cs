using MediatR;

namespace DeliverWholesale.Handler.Orders
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}