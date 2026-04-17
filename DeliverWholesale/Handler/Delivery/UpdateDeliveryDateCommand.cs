using MediatR;
using System;

namespace DeliverWholesale.Handler.Delivery
{
    public record UpdateDeliveryDateCommand(int Id, DateTime NewDate) : IRequest<bool>;
}