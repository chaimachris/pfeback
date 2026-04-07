using System.Collections.Generic;

namespace DeliverWholesale.DTOs
{
    public class OrderCreateDto
    {
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
    }
}