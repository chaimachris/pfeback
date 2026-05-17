namespace DeliverWholesale.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }

    public List<AchatLot> AchatLots { get; set; } = new();
}
