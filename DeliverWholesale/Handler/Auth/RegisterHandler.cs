using DeliverWholesale.Data;
using DeliverWholesale.Models;
using DeliverWholesale.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Auth
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public RegisterHandler(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1. Vérifier si email existe
            if (await _context.Users.AnyAsync(x => x.Email == request.Dto.Email))
                throw new Exception("Email existe déjà");

            // 2. Split prénom / nom
            var names = request.Dto.FullName.Split(' ');
            var prenom = names[0];
            var nom = names.Length > 1 ? names[1] : "";

            // 3. Générer token confirmation
            var confirmationToken = Guid.NewGuid().ToString();

            // 4. Créer user
            var user = new User
            {
                Prenom = prenom,
                Nom = nom,
                Email = request.Dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password),
                Role = Role.Client,
                IsEmailConfirmed = false,
                EmailConfirmationToken = confirmationToken
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 5. Lien confirmation email
            var confirmLink =
                $"http://localhost:5000/api/auth/confirm-email?email={user.Email}&token={confirmationToken}";

            // 6. Envoyer email
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirmation de votre compte DeliverWholesale",
                $@"
                    <h2>Bienvenue {prenom}</h2>
                    <p>Merci pour votre inscription.</p>
                    <p>Cliquez sur le lien ci-dessous pour confirmer votre email :</p>
                    <a href='{confirmLink}'>Confirmer mon compte</a>
                "
            );

            return "Compte créé avec succès. Vérifiez votre email pour activer votre compte.";
        }
    }
}