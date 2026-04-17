using MediatR;

namespace DeliverWholesale.Handler.Delivery
{
    public record GetTodayProductsQuery() : IRequest<object>;
}