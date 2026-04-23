using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Application.Features.Handler.Configurations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.API.Controllers
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