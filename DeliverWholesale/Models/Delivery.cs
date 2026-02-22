using System;

namespace DeliverWholesale.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string AdresseLivraison { get; set; }

        public DateTime? DateLivraisonPrevue { get; set; }
        public DateTime? DateLivraisonReelle { get; set; }

        public string Statut { get; set; } = "En préparation";

        public string NotesLivraison { get; set; }
    }
}