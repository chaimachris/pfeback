using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _mediator.Send(new RegisterCommand(dto));
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _mediator.Send(new LoginCommand(dto));
            return Ok(result);
        }
    }
}