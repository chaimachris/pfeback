using DeliverWholesale.Domain.Entities;

namespace DeliverWholesale.Application.DTOs
{
    public class StockDetailsDTO
    {
        public List<int> StockLotId { get; set; }
        public Produit Product { get; set; }
        public decimal QuantiteTotalRestante { get; set; }
        public List<Transaction> Transations { get; set; }
    }
}