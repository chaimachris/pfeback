using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.Features.Handler.Reclamations;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReclamationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public ReclamationController(
            IMediator mediator,
            ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        // POST: api/reclamation
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(
            CreateReclamationDto dto)
        {
            var result = await _mediator.Send(
                new CreateReclamationCommand(dto));

            return Ok(result);
        }

        // GET: api/reclamation/mes-reclamations
        [HttpGet("mes-reclamations")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MesReclamations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reclamations = await _context.Reclamations
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return Ok(reclamations);
        }

        // GET: api/reclamation
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reclamations = await _context.Reclamations
                .Include(r => r.Order)
                .Include(r => r.ResolvedByUser) // ← Un seul Include suffit
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            return Ok(reclamations);
        }

        // PUT: api/reclamation/{id}/traiter
        [HttpPut("{id}/traiter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Traiter(
            int id,
            TraiterReclamationDto dto)
        {
            var reclamation = await _context.Reclamations
                .FindAsync(id);

            if (reclamation == null)
                return NotFound();

            reclamation.Status = dto.Status;
            reclamation.ReponseAdmin = dto.ReponseAdmin;

            // ✅ CORRIGÉ : Même chose ici, on utilise ClaimTypes.NameIdentifier au lieu de "uid"
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Si résolue
            if (dto.Status == "Résolue")
            {
                reclamation.DateResolution = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(adminId))
                {
                    reclamation.ResolvedByUserId = int.Parse(adminId);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(reclamation);
        }

        // DELETE: api/reclamation/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var reclamation = await _context.Reclamations
                .FindAsync(id);

            if (reclamation == null)
                return NotFound();

            _context.Reclamations.Remove(reclamation);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Réclamation supprimée avec succès"
            });
        }
    }
}