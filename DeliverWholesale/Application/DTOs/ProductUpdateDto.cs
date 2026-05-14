using Microsoft.AspNetCore.Http;

namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class ProductUpdateDto
    {
        public string? Nom { get; set; }

        public string? Description { get; set; }

        public decimal? PrixAchat { get; set; }

        public int? CategorieId { get; set; }
        public int? NbUnite { get; set; }

        public IFormFile? Image { get; set; }
    }
}