using DeliverWholesale.Models;

namespace DeliverWholesale.DTOs
{
    public class UpdateDeliveryStatusDto
    {
        public DeliveryStatus Statut { get; set; }
        public DateTime? DateLivraisonReelle { get; set; }
    }
}