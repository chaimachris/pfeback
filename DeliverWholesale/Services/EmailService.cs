using DeliverWholesale.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DeliverWholesale.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SendGridSettings> settings,
                            ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        //  Méthode principale
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, "", body);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Email envoyé à {Email}", to);
            }
            else
            {
                _logger.LogError("❌ Erreur SendGrid: {Status}", response.StatusCode);
                throw new Exception("Erreur envoi email");
            }
        }

        //  Email bienvenue
        public async Task SendWelcomeEmailAsync(string to, string clientName)
        {
            var subject = "Bienvenue sur DeliverWholesale 🎉";
            var body = $"Bonjour {clientName}, votre compte est créé avec succès.";

            await SendEmailAsync(to, subject, body);
        }

        //  Statut commande
        public async Task SendOrderStatusEmailAsync(string to, string clientName, string status)
        {
            var subject = "Mise à jour commande";
            var body = $"Bonjour {clientName}, statut: {status}";

            await SendEmailAsync(to, subject, body);
        }

        //  Stock faible
        public async Task SendLowStockAlertAsync(string to, string productName, int currentStock)
        {
            var subject = "⚠️ Stock faible";
            var body = $"Produit {productName} stock: {currentStock}";

            await SendEmailAsync(to, subject, body);
        }
    }
}