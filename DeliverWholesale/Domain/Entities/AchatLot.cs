using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverWholesale.Domain.Entities;

public class AchatLot
{
    public int Id { get; set; }

    public int ProduitId { get; set; }
    public Produit Produit { get; set; }

    public DateTime DateAchat { get; set; }

    public int QuantiteAchetee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrixUnitaire { get; set; }

    // Backwards-compatible free-text supplier name. Keep until migration is complete.
    public string Fournisseur { get; set; }

    // New structured FK to Supplier. Nullable initially to avoid breaking existing data.
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public string NumeroLot { get; set; }

    public List<StockLot> StockLots { get; set; } = new();
}