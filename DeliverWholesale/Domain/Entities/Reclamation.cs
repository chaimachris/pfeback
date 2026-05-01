using System;
using System.ComponentModel.DataAnnotations;

namespace DeliverWholesale.Domain.Entities
{
    public class Reclamation
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Sujet { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public string Status { get; set; } = "En attente";

        public string? ReponseAdmin { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public DateTime? DateResolution { get; set; }

        // =========================
        // NOUVEAU CHAMP
        // =========================
        public int? ResolvedByUserId { get; set; }

        public User? ResolvedByUser { get; set; }
    }
}