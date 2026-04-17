using MediatR;
using DeliverWholesale.DTOs;

namespace DeliverWholesale.Handler.Auth
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