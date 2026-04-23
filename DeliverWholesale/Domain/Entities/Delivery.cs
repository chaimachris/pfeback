namespace DeliverWholesale.Domain.Entities
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string AdresseLivraison { get; set; }

        public DateTime DateLivraisonPrevue { get; set; }
        public DateTime? DateLivraisonReelle { get; set; }

        public DeliveryStatus Statut { get; set; } = DeliveryStatus.EnAttente;
    }

    public enum DeliveryStatus
    {
        EnAttente,
        Confirmee,
        Livree,
        Annulee
    }
}