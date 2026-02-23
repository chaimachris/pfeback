using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliverWholesale.Models
{
    public class StockLot
    {
        public int Id { get; set; }

        [Required]
        public int ProduitId { get; set; }
        public Produit Produit { get; set; }

        public int QuantiteAchetee { get; set; }
        public decimal PrixAchatLot { get; set; }

        public DateTime DateAchat { get; set; } = DateTime.UtcNow;
        public string Fournisseur { get; set; }

        public string Unite { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}