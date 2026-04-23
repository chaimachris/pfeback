using DeliverWholesale.Domain.Enums;

namespace DeliverWholesale.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public int StockLotId { get; set; }
        public StockLot StockLot { get; set; }

        public int? OrderDetailId { get; set; }
        public OrderDetail OrderDetail { get; set; }

        public TypeMouvement Type { get; set; }

        public int Quantite { get; set; }

        public DateTime DateMouvement { get; set; } = DateTime.UtcNow;
    }
}