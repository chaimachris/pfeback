using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverWholesale.Domain.Entities
{
    public class PrixVente
    {
        [Key]
        public int Id { get; set; }

        public int idP { get; set; }

        [ForeignKey("idP")]
        public Produit Produit { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Valeur { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}