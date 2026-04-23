using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record GetTodayProductsQuery() : IRequest<object>;
}