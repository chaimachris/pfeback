using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliverWholesale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // ========================
        //  GET PROFILE
        // ========================
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Nom,
                user.Prenom,
                user.Email,
                user.Adresse,
                user.Role
            });
        }

        // ========================
        //  UPDATE PROFILE
        // ========================
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = GetUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            user.Nom = dto.Nom;
            user.Prenom = dto.Prenom;
            user.Email = dto.Email;
            user.Adresse = dto.Adresse;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully");
        }

        // ========================
        //  CHANGE PASSWORD 
        // ========================
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = GetUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            //  vérifier ancien password avec BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return BadRequest("Old password is incorrect");

            //  hash nouveau password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok("Password updated successfully");
        }
        [HttpPut("update-delivery-address")]
        public async Task<IActionResult> UpdateDeliveryAddress(UpdateDeliveryAddressDto dto)
        {
            var userId = GetUserId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            
            user.AdresseLivraisonActive = dto.AdresseLivraisonActive;

            await _context.SaveChangesAsync();

            return Ok("Delivery address updated successfully");
        }
    }
}