using MediatR;
using System;

namespace DeliverWholesale.Application.Features.Handler.Deliveries
{
    public record UpdateDeliveryDateCommand(int Id, DateTime NewDate) : IRequest<bool>;
}