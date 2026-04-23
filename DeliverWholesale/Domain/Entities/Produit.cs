using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static DeliverWholesale.Domain.Entities.Produit;

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

        public decimal PrixVente { get; set; }

        public int SeuilAlerte { get; set; }

        public bool IsActive { get; set; } = true;

        
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }

        public enum TypePrix
        {
            Fix,
            Libre
        }
        public TypePrix TypeDePrix { get; set; } = TypePrix.Fix;
        public List<StockLot> StockLots { get; set; } = new List<StockLot>();

        [NotMapped]
        public int StockDisponible => StockLots.Sum(l => l.QuantiteRestante);

        [NotMapped]

        public bool AlerteStock => StockDisponible <= SeuilAlerte;
    }
}