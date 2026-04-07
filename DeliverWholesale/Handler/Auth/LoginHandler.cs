using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    public class LoginHandler : IRequestHandler<LoginCommand, object>
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwt;

        public LoginHandler(ApplicationDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<object> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
                throw new Exception("Identifiants invalides");

            var token = _jwt.GenerateToken(user);

            return new
            {
                token,
                role = user.Role.ToString()
            };
        }
    }
}