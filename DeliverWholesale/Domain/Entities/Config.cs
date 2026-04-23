namespace DeliverWholesale.Domain.Entities
{
    public class Config
    {
        public int Id { get; set; } = 1;
        public decimal MontantMinimumCommande { get; set; } = 100;
        public decimal ProfitPercentage { get; set; } = 20;
        public decimal FraisLivraison { get; set; } = 15;
        public int SeuilAlerteStockBas { get; set; } = 10;
    }
}