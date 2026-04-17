using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DeliverWholesale.Models
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
        public decimal Prix { get; set; }

        public int SeuilAlerte { get; set; }

        public bool IsActive { get; set; } = true;

        
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }

       
        public List<StockLot> StockLots { get; set; } = new List<StockLot>();

        [NotMapped]
        public int StockDisponible => StockLots.Sum(l => l.QuantiteRestante);

        [NotMapped]
        public bool AlerteStock => StockDisponible <= SeuilAlerte;
    }
}