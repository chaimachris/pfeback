using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record GetTodayClientsQuery() : IRequest<object>;
}