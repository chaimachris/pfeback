using System.Net;
using System.Net.Mail;

namespace DeliverWholesale.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    "your_email@gmail.com",
                    "your_app_password"
                ),
                EnableSsl = true
            };

            using var mail = new MailMessage
            {
                From = new MailAddress("your_email@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await smtp.SendMailAsync(mail);
        }
    }
}