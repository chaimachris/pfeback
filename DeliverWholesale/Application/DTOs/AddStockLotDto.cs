namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class AddStockLotDto
    {
        public int AchatLotId { get; set; }
        public int Quantite { get; set; }
        public decimal PrixAchatTotal { get; set; }
        public string? Fournisseur { get; set; }
        public string? Unite { get; set; }


    }
}