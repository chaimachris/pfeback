using DeliverWholesale.Models;

public class StockLot
{
    public int Id { get; set; }

    public int AchatLotId { get; set; }
    public AchatLot AchatLot { get; set; }

    public int QuantiteRestante { get; set; }

    public DateTime DateReception { get; set; } = DateTime.UtcNow;

   
    public List<LotCommande> LotCommandes { get; set; } = new();

    public List<Transaction> Transactions { get; set; } = new();
}