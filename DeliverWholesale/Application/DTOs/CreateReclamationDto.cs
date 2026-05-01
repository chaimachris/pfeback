namespace DeliverWholesale.Application.DTOs
{
    public class CreateReclamationDto
    {
        public int OrderId { get; set; }

        public string Sujet { get; set; }

        public string Description { get; set; }
    }
}