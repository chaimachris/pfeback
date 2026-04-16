using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Delivery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost]
        public async Task<IActionResult> Create(CreateDeliveryDto dto)
        {
            var result = await _mediator.Send(new CreateDeliveryCommand(dto));
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetDeliveriesQuery());
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateDeliveryStatusDto dto)
        {
            var updated = await _mediator.Send(new UpdateDeliveryStatusCommand(id, dto));

            if (!updated)
                return NotFound();

            return Ok("Statut livraison mis à jour");
        }
    }
}