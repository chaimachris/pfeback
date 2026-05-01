using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Reclamations
{
    public record CreateReclamationCommand(CreateReclamationDto Dto)
        : IRequest<Reclamation>;
}
