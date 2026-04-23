using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverWholesale.Domain.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProduitId { get; set; }
        public Produit Produit { get; set; }

        public int Quantite { get; set; }

        public decimal PrixUnitaire { get; set; }

        [NotMapped]
        public decimal SousTotal => Quantite * PrixUnitaire;
    }
}