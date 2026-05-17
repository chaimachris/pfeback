using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeliverWholesale.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalProduits { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FraisLivraison { get; set; }

        [NotMapped]
        public decimal TotalFinal => TotalProduits + FraisLivraison;

        public StatutOrder Statut { get; set; } = StatutOrder.EnAttente;

        public Delivery? Delivery { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }

    public enum StatutOrder
    {
        EnAttente,
        Confirmee,
        Livree,
        Annulee
    }
}