using MediatR;
using DeliverWholesale.Application.DTOs.DTOs;

namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class LoginCommand : IRequest<object>
    {
        public LoginDto Dto { get; set; }

        public LoginCommand(LoginDto dto)
        {
            Dto = dto;
        }
    }
}