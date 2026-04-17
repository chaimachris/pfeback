using MediatR;

namespace DeliverWholesale.Handler.Delivery
{
    public record GetTodayClientsQuery() : IRequest<object>;
}