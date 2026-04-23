using DeliverWholesale.Domain.Entities;

public class LotCommande
{
    public int Id { get; set; }

    public int StockLotId { get; set; }
    public StockLot StockLot { get; set; }

    public int OrderDetailId { get; set; }
    public OrderDetail OrderDetail { get; set; }

    public int QuantitePrelevee { get; set; }
}