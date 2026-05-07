using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DeliverWholesale.Domain.Entities
{
    public class Produit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nom { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal PrixAchat { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal PrixVente { get; set; }

        public int NbUnite { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategorieId { get; set; }

        public Categorie Categorie { get; set; }

        // ✅ IMAGE
        public string? ImageUrl { get; set; }

        public enum TypePrix
        {
            Fix,
            Libre
        }

        public TypePrix TypeDePrix { get; set; } = TypePrix.Fix;

        public List<StockLot> StockLots { get; set; } = new();

        [NotMapped]
        public int StockDisponible => StockLots.Sum(l => l.QuantiteRestante);
    }
}