using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using DeliverWholesale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;
        private readonly ApplicationDbContext _context;

        public OrdersController(OrderService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = await _service.CreateOrder(userId, dto);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Produit)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Produit)
                .FirstOrDefaultAsync(o => o.Id == id && (o.UserId == userId || User.IsInRole("Admin")));
            if (order == null) return NotFound();
            return Ok(order);
        }

      

        [HttpDelete("{id}")]
        public async Task<IActionResult> Annuler(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.UserId != userId || order.Statut != StatutOrder.EnAttente) return BadRequest("Impossible d'annuler");

            order.Statut = StatutOrder.Annulee;
            await _service.RevertStock(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Produit)
                .ToListAsync();
            return Ok(orders);
        }
    }
}