using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverWholesale.Infrastructure.Services
{
    public record AllocateItem(int ProduitId, int OrderDetailId, int Quantite);
    public record LotAllocationPart(int StockLotId, int OrderDetailId, int QuantitePrelevee);

    public interface IInventoryService
    {
        Task<Dictionary<int, List<LotAllocationPart>>> AllocateOrderStockAsync(int orderId, IEnumerable<AllocateItem> items);

        Task<int> ReceivePurchaseStockAsync(int produitId, int quantiteAchetee, decimal prixUnitaire, string? numeroLot = null, int? supplierId = null);

        Task<int> ReceiveStockForAchatLotAsync(int achatLotId, int quantite, decimal prixAchatTotal, string? fournisseur = null);

        Task AdjustStockAsync(int stockLotId, int delta, TypeMouvement type, int? orderDetailId = null);

        Task RevertOrderStockAsync(int orderId);
        Task RevertOrderStockTransactionalAsync(int orderId);
        // Data-migration helper: create Supplier rows from existing AchatLot.Fournisseur values and link them.
        Task<int> MigrateSuppliersFromAchatLotsAsync();

        Task<(bool Sufficient, Dictionary<int,int> Shortages)> ValidateAvailabilityAsync(IEnumerable<(int ProduitId, int Quantite)> items);
    }
}
