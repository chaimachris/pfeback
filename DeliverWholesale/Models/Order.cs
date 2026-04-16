using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverWholesale.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }



        public DateTime DateCommande { get; set; } = DateTime.UtcNow;

        public decimal TotalProduits { get; set; }

        public decimal FraisLivraison { get; set; }

        [NotMapped]
        public decimal TotalFinal => TotalProduits + FraisLivraison;

        public StatutOrder Statut { get; set; } = StatutOrder.EnAttente;

        public bool IsDeleted { get; set; }

        public Delivery Delivery { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum StatutOrder
    {
        EnAttente,
        Confirmee,
        Preparation,
        Expediee,
        livree,
        Annulee
    }
}