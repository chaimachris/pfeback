using DeliverWholesale.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class StockLot
{
    public int Id { get; set; }

    public int AchatLotId { get; set; }
    public AchatLot AchatLot { get; set; }

    public int QuantiteRestante { get; set; }

    public DateTime DateReception { get; set; } = DateTime.UtcNow;

    // Optional expiration date; null means no expiration (keep backward compatible)
    public DateTime? ExpirationDate { get; set; }

    public Produit Produit { get; set; }
    public int ProduitId { get; set; }
   
    public List<LotCommande> LotCommandes { get; set; } = new();

    public List<Transaction> Transactions { get; set; } = new();

    // Concurrency token to prevent lost updates
    [Timestamp]
    public byte[] RowVersion { get; set; }
}