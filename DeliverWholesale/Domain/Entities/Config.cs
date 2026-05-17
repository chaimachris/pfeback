using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverWholesale.Domain.Entities
{
    public class Config
    {
        public int Id { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantMinimumCommande { get; set; } = 100;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProfitPercentage { get; set; } = 20;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FraisLivraison { get; set; } = 15;

        public int SeuilAlerteStockBas { get; set; } = 10;
    }
}