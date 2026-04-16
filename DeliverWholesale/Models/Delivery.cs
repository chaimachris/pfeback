using System;

namespace DeliverWholesale.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string AdresseLivraison { get; set; }

        public DateOnly DateLivraisonPrevue { get; set; }
        public DateOnly? DateLivraisonReelle { get; set; }
        public DeliveryStatus Statut { get; set; } = DeliveryStatus.EnAttente;

    }
    public enum DeliveryStatus
    {
        EnAttente,
        Confirmee,
        Preparation,
        Expediee,
        livree,
        Annulee
    }

}