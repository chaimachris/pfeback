using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using DeliverWholesale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PricingService _pricing;

        public ProductsController(ApplicationDbContext context, PricingService pricing)
        {
            _context = context;
            _pricing = pricing;
        }

        [HttpGet]
        [Authorize] 
        public async Task<IActionResult> Get()
            => Ok(await _context.Produits.Where(p => p.IsActive).Include(p => p.Categorie).ToListAsync());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var config = await _context.Configs.FirstAsync();
            var product = new Produit
            {
                Nom = dto.Nom,
                Description = dto.Description,
                PrixAchat = dto.PrixAchat,
                PrixVente = _pricing.CalculerPrixVente(dto.PrixAchat, config.ProfitPercentage),
                CategorieId = dto.CategorieId,
                IsActive = true
            };

            _context.Produits.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var product = await _context.Produits.FindAsync(id);
            if (product == null) return NotFound();

            product.Nom = dto.Nom ?? product.Nom;
            product.Description = dto.Description ?? product.Description;
            product.PrixAchat = dto.PrixAchat ?? product.PrixAchat;
            product.PrixVente = _pricing.CalculerPrixVente(product.PrixAchat, (await _context.Configs.FirstAsync()).ProfitPercentage);
            product.CategorieId = dto.CategorieId ?? product.CategorieId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Produits.FindAsync(id);
            if (product == null) return NotFound();

            _context.Produits.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}