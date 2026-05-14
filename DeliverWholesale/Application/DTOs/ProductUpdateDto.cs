using Microsoft.AspNetCore.Http;

namespace DeliverWholesale.Application.DTOs
{
    public class ProductUpdateDto
    {
        public string? libelle { get; set; }

        public string? Description { get; set; }

        public decimal? NouveauPrixVente { get; set; }   // crée une nouvelle entrée PrixVente si renseigné

        public int? idCategorie { get; set; }

        public int? seuil { get; set; }

        public bool? prixModifiable { get; set; }

        public IFormFile? Image { get; set; }
    }
}