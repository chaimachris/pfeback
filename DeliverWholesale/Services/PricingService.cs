namespace DeliverWholesale.Services
{
    public class PricingService
    {
        public decimal CalculerPrixVente(decimal prixAchat, decimal profitPercentage)
        {
            return prixAchat * (1 + profitPercentage / 100);
        }
    }
}