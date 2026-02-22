using DeliverWholesale.Data;
using DeliverWholesale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _context.Configs.FirstOrDefaultAsync(); 
            if (config == null)
            {
                config = new Config { Id = 1, MontantMinimumCommande = 100, FraisLivraison = 15, ProfitPercentage = 20, SeuilAlerteStockBas = 10 };
                _context.Configs.Add(config);
                await _context.SaveChangesAsync();
            }
            return Ok(config);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfig([FromBody] Config updated)
        {
            var config = await _context.Configs.FindAsync(1);
            if (config == null) return NotFound();

            config.MontantMinimumCommande = updated.MontantMinimumCommande;
            config.FraisLivraison = updated.FraisLivraison;
            config.ProfitPercentage = updated.ProfitPercentage;
            config.SeuilAlerteStockBas = updated.SeuilAlerteStockBas;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}