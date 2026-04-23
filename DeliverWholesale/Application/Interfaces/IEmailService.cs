namespace DeliverWholesale.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string to, string clientName, string confirmLink);

        Task SendOrderStatusEmailAsync(string to, string clientName, string status);
        //Task SendLowStockAlertAsync(string to, string productName, int currentStock);
        //Task SendWelcomeEmailAsync(string to, string clientName);
    }
}