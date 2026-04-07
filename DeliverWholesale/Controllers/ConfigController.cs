using DeliverWholesale.Handler.Config;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/config")]
    [Authorize(Roles = "Admin")]
    public class ConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _mediator.Send(new GetConfigQuery());
            return Ok(config);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfig([FromBody] Config updated)
        {
            var success = await _mediator.Send(new UpdateConfigCommand(updated));

            if (!success)
                return NotFound("Configuration introuvable");

            return NoContent();
        }
    }
}