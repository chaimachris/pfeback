namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class CreateDeliveryDto
    {
        public int OrderId { get; set; }
        public string AdresseLivraison { get; set; }

        public DateTime DateLivraisonPrevue { get; set; }
    }
}