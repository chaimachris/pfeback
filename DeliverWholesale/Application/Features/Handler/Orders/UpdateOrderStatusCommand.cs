using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Statut { get; set; }

        public UpdateOrderStatusCommand(int id, string statut)
        {
            Id = id;
            Statut = statut;
        }
    }
}