using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}