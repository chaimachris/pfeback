namespace DeliverWholesale.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendOrderStatusEmailAsync(string to, string clientName, string status);
        Task SendLowStockAlertAsync(string to, string productName, int currentStock);
        Task SendWelcomeEmailAsync(string to, string clientName);
    }
}