using System;

namespace DeliverWholesale.Application.DTOs
{
    public class PrixVenteCreateDto
    {
        public int idP { get; set; }
        public decimal Valeur { get; set; }
    }

    public class PrixVenteReadDto
    {
        public int Id { get; set; }
        public int idP { get; set; }
        public decimal Valeur { get; set; }
        public DateTime Date { get; set; }
    }
}