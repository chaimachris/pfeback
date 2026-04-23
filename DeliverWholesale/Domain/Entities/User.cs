using System.Collections.Generic;

namespace DeliverWholesale.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }

        public string? Adresse { get; set; }
        public string? AdresseLivraisonActive { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; } = Role.Client;

        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public enum Role
    {
        Client,
        Admin
    }
}