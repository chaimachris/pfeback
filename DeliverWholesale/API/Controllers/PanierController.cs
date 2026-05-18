using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Application.Features.Handler.Orders;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.API.Controllers
{
    [ApiController]
    [Route("api/panier")]
    [Authorize]
    public class PanierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public PanierController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationException("Utilisateur non authentifie.");
            return int.Parse(userId);
        }

        private async Task<PanierDto> BuildPanierDtoAsync(int userId)
        {
            var panier = await _context.Paniers
                .Include(p => p.Items)
                    .ThenInclude(i => i.Produit)
                        .ThenInclude(p => p.PrixVentes)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null)
            {
                panier = new Panier { UserId = userId };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
                return new PanierDto { UserId = userId };
            }

            var items = panier.Items.Select(i =>
            {
                var prix = i.Produit?.PrixVenteActuel ?? 0m;
                return new PanierItemDto
                {
                    ProduitId = i.ProduitId,
                    Libelle = i.Produit?.libelle ?? string.Empty,
                    Quantite = i.Quantite,
                    PrixUnitaire = prix,
                    SousTotal = prix * i.Quantite,
                    ImageUrl = i.Produit?.ImageUrl
                };
            }).ToList();

            return new PanierDto
            {
                UserId = userId,
                Items = items,
                TotalPrix = items.Sum(i => i.SousTotal)
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetPanier()
        {
            var userId = GetUserId();
            var dto = await BuildPanierDtoAsync(userId);
            return Ok(dto);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToPanierDto dto)
        {
            if (dto.Quantite <= 0)
                return BadRequest("Quantite invalide.");

            var userId = GetUserId();

            var product = await _context.Produits
                .Include(p => p.PrixVentes)
                .FirstOrDefaultAsync(p => p.idP == dto.ProduitId && p.IsActive);

            if (product == null)
                return NotFound("Produit introuvable.");

            var panier = await _context.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null)
            {
                panier = new Panier { UserId = userId };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            var existingItem = panier.Items.FirstOrDefault(i => i.ProduitId == dto.ProduitId);
            if (existingItem == null)
            {
                panier.Items.Add(new PanierItem
                {
                    ProduitId = dto.ProduitId,
                    Quantite = dto.Quantite
                });
            }
            else
            {
                existingItem.Quantite += dto.Quantite;
            }

            panier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(await BuildPanierDtoAsync(userId));
        }

        [HttpPut("items/{produitId}")]
        public async Task<IActionResult> UpdateItem(int produitId, [FromBody] UpdatePanierItemDto dto)
        {
            if (dto.Quantite < 0)
                return BadRequest("Quantite invalide.");

            var userId = GetUserId();

            var panier = await _context.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null)
                return NotFound("Panier introuvable.");

            var item = panier.Items.FirstOrDefault(i => i.ProduitId == produitId);
            if (item == null)
                return NotFound("Produit introuvable dans le panier.");

            if (dto.Quantite == 0)
            {
                _context.PanierItems.Remove(item);
            }
            else
            {
                item.Quantite = dto.Quantite;
            }

            panier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(await BuildPanierDtoAsync(userId));
        }

        [HttpDelete("items/{produitId}")]
        public async Task<IActionResult> RemoveItem(int produitId)
        {
            var userId = GetUserId();

            var panier = await _context.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null)
                return NotFound("Panier introuvable.");

            var item = panier.Items.FirstOrDefault(i => i.ProduitId == produitId);
            if (item == null)
                return NotFound("Produit introuvable dans le panier.");

            _context.PanierItems.Remove(item);
            panier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(await BuildPanierDtoAsync(userId));
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearPanier()
        {
            var userId = GetUserId();

            var panier = await _context.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null)
                return NotFound("Panier introuvable.");

            if (panier.Items.Any())
            {
                _context.PanierItems.RemoveRange(panier.Items);
                panier.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(await BuildPanierDtoAsync(userId));
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();

            var panier = await _context.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (panier == null || !panier.Items.Any())
                return BadRequest("Panier vide.");

            var orderDto = new OrderCreateDto
            {
                Items = panier.Items.Select(i => new OrderItemDto
                {
                    ProduitId = i.ProduitId,
                    Quantite = i.Quantite
                }).ToList()
            };

            var orderId = await _mediator.Send(new CreateOrderCommand(orderDto));

            _context.PanierItems.RemoveRange(panier.Items);
            panier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Commande creee", OrderId = orderId });
        }
    }
}
