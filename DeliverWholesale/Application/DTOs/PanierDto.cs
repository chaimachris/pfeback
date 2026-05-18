using System.Collections.Generic;

namespace DeliverWholesale.Application.DTOs
{
    public class AddToPanierDto
    {
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
    }

    public class UpdatePanierItemDto
    {
        public int Quantite { get; set; }
    }

    public class PanierItemDto
    {
        public int ProduitId { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal SousTotal { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class PanierDto
    {
        public int UserId { get; set; }
        public decimal TotalPrix { get; set; }
        public List<PanierItemDto> Items { get; set; } = new();
    }
}
