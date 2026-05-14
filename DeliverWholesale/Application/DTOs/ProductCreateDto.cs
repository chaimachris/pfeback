using Microsoft.AspNetCore.Http;

namespace DeliverWholesale.Application.DTOs
{
    public class ProductCreateDto
    {
        public string libelle { get; set; }

        public string Description { get; set; }

        public decimal PrixVente { get; set; }          // prix initial → créer entrée PrixVente

        public int idCategorie { get; set; }

        public int NbUnite { get; set; }

        public int seuil { get; set; }

        public bool prixModifiable { get; set; }

        public IFormFile? Image { get; set; }
    }
}