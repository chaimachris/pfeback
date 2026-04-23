using MediatR;
using DeliverWholesale.Application.DTOs.DTOs;

namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class RegisterCommand : IRequest<string>
    {
        public RegisterDto Dto { get; set; }

        public RegisterCommand(RegisterDto dto)
        {
            Dto = dto;
        }
    }
}