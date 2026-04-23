using DeliverWholesale.Domain.Entities;

namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class UpdateDeliveryStatusDto
    {
        public DeliveryStatus Statut { get; set; }
        public DateTime? DateLivraisonReelle { get; set; }
    }
}