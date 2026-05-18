using System;
using System.Collections.Generic;

namespace DeliverWholesale.Domain.Entities
{
    public class Panier
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<PanierItem> Items { get; set; } = new();
    }
}
