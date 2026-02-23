using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStockOverview()
        {
            var stock = await _context.Produits
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    ProduitId = p.Id,
                    Nom = p.Nom,
                    StockActuel = p.StockActuel,
                    PrixVente = p.PrixVente,
                    DernierAchat = p.StockLots
                        .OrderByDescending(l => l.DateAchat)
                        .Select(l => new { l.DateAchat, l.QuantiteAchetee })
                        .FirstOrDefault()
                })
                .OrderBy(x => x.StockActuel)
                .ToListAsync();

            return Ok(stock);
        }


        [HttpPost("add-lot")]
        public async Task<IActionResult> AddStockLot([FromBody] AddStockLotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Produits.FindAsync(dto.ProduitId);
            if (product == null)
                return NotFound("Produit non trouvé");

            var lot = new StockLot
            {
                ProduitId = dto.ProduitId,
                QuantiteAchetee = dto.Quantite,
                PrixAchatLot = dto.PrixAchatTotal,
                Fournisseur = dto.Fournisseur,
                DateAchat = DateTime.UtcNow,
                Unite = dto.Unite ?? "unité"
            };

            // Ajoute transaction entrée
            var transaction = new Transaction
            {
                StockLot = lot,
                Type = TypeMouvement.Entree,
                Quantite = dto.Quantite,
                DateMouvement = DateTime.UtcNow
            };
            lot.Transactions.Add(transaction);

            _context.StockLots.Add(lot);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockOverview), new { id = lot.Id }, lot);
        }

        
    }
}