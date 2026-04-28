using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly JwtService _jwtService;

        public LogoutHandler(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Token))
                _jwtService.RevokeToken(request.Token);

            return await Task.FromResult(true);
        }
    }
}
