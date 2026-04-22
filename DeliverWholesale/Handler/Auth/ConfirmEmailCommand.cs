using MediatR;

namespace DeliverWholesale.Handler.Auth
{
    public class ConfirmEmailCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public string Token { get; set; }

        public ConfirmEmailCommand(string email, string token)
        {
            Email = email;
            Token = token;
        }
    }
}