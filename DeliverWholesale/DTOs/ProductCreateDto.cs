namespace DeliverWholesale.DTOs
{
    public class ProductCreateDto
    {
        public string Nom { get; set; }
        public string Description { get; set; }
        public decimal PrixAchat { get; set; }
        public int CategorieId { get; set; }
    }
}