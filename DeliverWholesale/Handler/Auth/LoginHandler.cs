using DeliverWholesale.Data;
using DeliverWholesale.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Auth
{
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
            //  chercher user
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Dto.Email);

            //  vérifier existence + password
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
                throw new Exception("Email ou mot de passe incorrect");

            // vérifier email confirmé
            if (!user.IsEmailConfirmed)
                throw new Exception("Veuillez confirmer votre email");

            //  générer token JWT
            var token = _jwt.GenerateToken(user);

            //  retour réponse
            return new
            {
                token,
                role = user.Role.ToString(),
                email = user.Email,
                fullName = user.Prenom + " " + user.Nom
            };
        }
    }
}