using DeliverWholesale.Application.DTOs;
using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Application.Features.Handler.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            try
            {
                var orderId = await _mediator.Send(new CreateOrderCommand(dto));

                return Ok(new
                {
                    Message = "Commande créée avec succès",
                    OrderId = orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Erreur lors de la création de la commande",
                    Error = ex.Message
                });
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery { Id = id });

            if (result == null)
                return NotFound("Commande introuvable");

            return Ok(result);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteOrderCommand { Id = id });

            if (!result)
                return NotFound("Commande introuvable");

            return Ok(new { Message = "Commande supprimée" });
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand(id, dto.Statut));

            if (!result)
                return NotFound("Commande introuvable");

            return Ok(new { message = "Statut mis à jour" });
        }
    }
}