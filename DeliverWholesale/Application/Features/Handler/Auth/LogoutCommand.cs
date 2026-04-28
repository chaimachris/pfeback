using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class LogoutCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;  
    }
}
