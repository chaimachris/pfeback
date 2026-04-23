using System.ComponentModel.DataAnnotations;

namespace DeliverWholesale.Application.DTOs.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}