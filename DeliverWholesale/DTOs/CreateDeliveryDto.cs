namespace DeliverWholesale.DTOs
{
    public class CreateDeliveryDto
    {
        public int OrderId { get; set; }
        public string AdresseLivraison { get; set; }
        public DateOnly DateLivraisonPrevue { get; set; }
        public DateOnly DateLivraisonReelle { get; set; }
    }
}