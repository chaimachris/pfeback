using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliverWholesale.Domain.Entities
{
    public class Categorie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int? ParentId { get; set; }
        public Categorie Parent { get; set; }

        public List<Categorie> SousCategories { get; set; } = new List<Categorie>();
        public List<Produit> Produits { get; set; } = new List<Produit>();
    }
}