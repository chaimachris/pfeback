using MediatR;
using DeliverWholesale.DTOs;

namespace DeliverWholesale.Handler.Auth
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