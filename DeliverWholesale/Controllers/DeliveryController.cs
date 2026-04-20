using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Delivery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // =========================
        // CREATE DELIVERY
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateDeliveryDto dto)
        {
            var result = await _mediator.Send(new CreateDeliveryCommand(dto));
            return Ok(result);
        }

        // =========================
        // GET ALL DELIVERIES
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetDeliveriesQuery());
            return Ok(result);
        }

        // =========================
        // UPDATE STATUS
        // =========================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateDeliveryStatusDto dto)
        {
            var updated = await _mediator.Send(new UpdateDeliveryStatusCommand(id, dto));

            if (!updated)
                return NotFound();

            return Ok("Statut livraison mis à jour");
        }

        // =========================
        //  PRODUCTS TODAY
        // =========================
        [HttpGet("today/products")]
        public async Task<IActionResult> GetTodayProducts()
        {
            var result = await _mediator.Send(new GetTodayProductsQuery());
            return Ok(result);
        }

        // =========================
        //  CLIENTS TODAY
        // =========================
        [HttpGet("today/clients")]
        public async Task<IActionResult> GetTodayClients()
        {
            var result = await _mediator.Send(new GetTodayClientsQuery());
            return Ok(result);
        }

        // =========================
        //  UPDATE DELIVERY DATE
        // =========================
        [HttpPut("{id}/date")]
        public async Task<IActionResult> UpdateDate(int id, [FromBody] DateTime newDate)
        {
            var result = await _mediator.Send(new UpdateDeliveryDateCommand(id, newDate));

            if (!result)
                return NotFound();

            return Ok("Date de livraison mise à jour");
        }
    }
}