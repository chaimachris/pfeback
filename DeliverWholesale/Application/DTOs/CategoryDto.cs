namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class CategoryDto
    {
        public required string Nom { get; set; }
        public required string Description { get; set; }
        public int? ParentId { get; set; }
    }
}