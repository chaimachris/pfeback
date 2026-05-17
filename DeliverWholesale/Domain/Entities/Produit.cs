using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DeliverWholesale.Domain.Entities
{
    [Table("Produits")]
    public class Produit
    {
        [Key]
        public int idP { get; set; }

        [Required]
        [StringLength(150)]
        public string libelle { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int seuil { get; set; } = 0;

        public bool prixModifiable { get; set; } = false;

        public int idCategorie { get; set; }

        [ForeignKey("idCategorie")]
        public Categorie Categorie { get; set; }

        public int NbUnite { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }

        public List<StockLot> StockLots { get; set; } = new();

        public List<PrixVente> PrixVentes { get; set; } = new();

        [NotMapped]
        public int StockDisponible => StockLots.Sum(l => l.QuantiteRestante);

        [NotMapped]
        public decimal? PrixVenteActuel => PrixVentes
            .OrderByDescending(p => p.Date)
            .FirstOrDefault()?.Valeur;
    }
}