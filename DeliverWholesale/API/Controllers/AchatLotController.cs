using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Application.Features.Handler.AchatLots;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DeliverWholesale.Application.Features.Handler.AchatLots;

namespace DeliverWholesale.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchatLotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AchatLotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAchat(CreateAchatLotCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(new
            {
                message = "Achat créé avec succès",
                AchatLotId = id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAchatLotsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetAchatLotByIdQuery(id));

            if (result == null)
                return NotFound("Achat introuvable");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteAchatLotCommand { Id = id });

            if (!success)
                return NotFound("Achat introuvable");

            return Ok("Achat supprimé avec succès");
        }
    }
}