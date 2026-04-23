namespace DeliverWholesale.Domain.Entities;

public class AchatLot
{
    public int Id { get; set; }

    public int ProduitId { get; set; }
    public Produit Produit { get; set; }

    public DateTime DateAchat { get; set; }

    public int QuantiteAchetee { get; set; }

    public decimal PrixUnitaire { get; set; }

    public string Fournisseur { get; set; }

    public string NumeroLot { get; set; }

    public List<StockLot> StockLots { get; set; } = new();
}