using Azure;
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
        private async Task SendEmailAsync(string to, string subject, string body)
        {

            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, "", body);
            SendGrid.Response response = null;
            try
            {
                response = await client.SendEmailAsync(msg);
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
            catch (Exception e)
            {
                _logger.LogError("❌ Erreur SendGrid: {Status}", response.StatusCode);
                throw new Exception("Erreur envoi email");
                throw;
            }

            
        }

        //  Email bienvenue
        public async Task SendWelcomeEmailAsync(string to, string clientName,string confirmLink)
        {
            var subject = "Confirmez votre compte DeliverWholesale";

            var body = $@"
            <div style='font-family:Arial; max-width:600px; margin:auto;'>
                <h2 style='color:#2E75B6;'>Bienvenue {clientName} 👋</h2>

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
        //public async Task SendLowStockAlertAsync(string to, string productName, int currentStock)
        //{
        //    var subject = "⚠️ Stock faible";
        //    var body = $"Produit {productName} stock: {currentStock}";

        //    await SendEmailAsync(to, subject, body);
        //}
    }
}