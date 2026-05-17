using DeliverWholesale.Application.Interfaces;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public RegisterHandler(
            ApplicationDbContext context,
            IEmailService emailService,
            IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLower();

            
            if (await _context.Users.AnyAsync(x => x.Email == email, cancellationToken))
                throw new ApplicationException("Email déjà utilisé"); //To Fix

           
            var fullName = request.Dto.FullName.Trim();

            var index = fullName.IndexOf(' ');

            var prenom = index == -1 ? fullName : fullName.Substring(0, index);
            var nom = index == -1 ? "" : fullName.Substring(index + 1);

            //  Générer token
            var token = Guid.NewGuid().ToString();

            //  Créer user
            var user = new User
            {
                Prenom = prenom,
                Nom = nom,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password),
                Role = Role.Client,
                IsEmailConfirmed = true, // true
                EmailConfirmationToken = token // null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            //  Lien confirmation (Angular)
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";

            var confirmLink =
                $"{frontendUrl}/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            

            //  Envoi email sécurisé
            try
            {
                await _emailService.SendWelcomeEmailAsync(email, prenom, confirmLink);
            }
            catch
            {
                //  ne bloque pas inscription si email échoue
                return "Compte créé mais email non envoyé.";
            }

            return "Compte créé avec succès. Vérifiez votre email.";
        }
    }
}