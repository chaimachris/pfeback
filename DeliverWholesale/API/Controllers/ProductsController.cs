using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Application.Features.Handler.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
        {
            var result = await _mediator.Send(new CreateProductCommand(dto));
            return Ok(result);
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return Ok(products);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] ProductUpdateDto dto)
        {
            var result = await _mediator.Send(
                new UpdateProductCommand(id, dto));

            if (!result)
                return NotFound();

            return Ok();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(
                new DeleteProductCommand(id));

            if (!result)
                return NotFound();

            return Ok();
        }

        // ──────────────────────────────────────────────────
        // 🆕 ENDPOINTS PRIX DE VENTE
        // ──────────────────────────────────────────────────

        // POST api/products/{id}/prix
        // Permet d'ajouter un nouveau prix à un produit (historique des prix)
        [HttpPost("{id}/prix")]
        public async Task<IActionResult> AddPrix(int id, [FromBody] PrixVenteCreateDto dto)
        {
            // On s'assure que l'idP correspond bien à l'ID de la route URL
            dto.idP = id;

            var result = await _mediator.Send(new CreatePrixVenteCommand(dto));
            return Ok(new { PrixVenteId = result });
        }

        // GET api/products/{id}/prix
        // Permet de récupérer l'historique des prix d'un produit
        [HttpGet("{id}/prix")]
        public async Task<IActionResult> GetPrix(int id)
        {
            var result = await _mediator.Send(new GetPrixVenteByProduitQuery(id));
            return Ok(result);
        }
    }
}