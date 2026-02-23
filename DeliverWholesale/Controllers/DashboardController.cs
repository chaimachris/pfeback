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
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = new
            {
                TotalProduits = await _context.Produits.CountAsync(p => p.IsActive),
                TotalCommandes = await _context.Orders.CountAsync(),
                CommandesEnAttente = await _context.Orders.CountAsync(o => o.Statut == StatutOrder.EnAttente),
                StockBas = await _context.Produits.CountAsync(p => p.StockActuel <= 10 && p.IsActive),
                StockNegatif = await _context.Produits.CountAsync(p => p.StockActuel < 0 && p.IsActive),
                ChiffreAffairesMois = await _context.Orders
                    .Where(o => o.DateCommande >= DateTime.UtcNow.AddMonths(-1) && o.Statut == StatutOrder.Livree)
                    .SumAsync(o => o.TotalFinal)
            };

            return Ok(stats);
        }

        [HttpGet("alertes")]
        public async Task<IActionResult> GetAlertes()
        {
            var config = await _context.Configs.FirstOrDefaultAsync();
            int seuil = config?.SeuilAlerteStockBas ?? 10;

            var alertes = await _context.Produits
                .Where(p => p.IsActive && p.StockActuel <= seuil)
                .Select(p => new
                {
                    Produit = p.Nom,
                    Stock = p.StockActuel,
                    EstCritique = p.StockActuel < 0
                })
                .ToListAsync();

            return Ok(alertes);
        }
    }
}