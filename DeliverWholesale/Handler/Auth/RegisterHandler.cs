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

            // ========================
            //  Vérifier email
            // ========================
            if (await _context.Users.AnyAsync(x => x.Email == email, cancellationToken))
                throw new ApplicationException("Email déjà utilisé");

            // ========================
            //  Split nom
            // ========================
            var names = request.Dto.FullName.Trim().Split(' ', 2);
            var prenom = names[0];
            var nom = names.Length > 1 ? names[1] : "";

            // ========================
            //  Générer token
            // ========================
            var token = Guid.NewGuid().ToString();

            // ========================
            //  Créer user
            // ========================
            var user = new User
            {
                Prenom = prenom,
                Nom = nom,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password),
                Role = Role.Client,
                IsEmailConfirmed = false,
                EmailConfirmationToken = token
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // ========================
            //  Lien confirmation (Angular)
            // ========================
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";

            var confirmLink =
                $"{frontendUrl}/confirm-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            // ========================
            //  Email HTML PRO
            // ========================
            var subject = "Confirmez votre compte DeliverWholesale";

            var body = $@"
            <div style='font-family:Arial; max-width:600px; margin:auto;'>
                <h2 style='color:#2E75B6;'>Bienvenue {prenom} 👋</h2>

                <p>Merci pour votre inscription sur <strong>DeliverWholesale</strong>.</p>

                <p>Veuillez confirmer votre email :</p>

                <div style='text-align:center; margin:30px 0;'>
                    <a href='{confirmLink}'
                       style='background:#2E75B6;color:white;padding:12px 20px;
                              text-decoration:none;border-radius:6px;'>
                        Confirmer mon compte
                    </a>
                </div>

                <p style='font-size:12px;color:gray;'>
                    Ce lien expire dans le futur.
                </p>
            </div>";

            // ========================
            //  Envoi email sécurisé
            // ========================
            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
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