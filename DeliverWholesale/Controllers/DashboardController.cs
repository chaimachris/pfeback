using DeliverWholesale.Handler.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(stats);
        }

        [HttpGet("alertes")]
        public async Task<IActionResult> GetAlertes()
        {
            var alertes = await _mediator.Send(new GetDashboardAlertesQuery());
            return Ok(alertes);
        }
    }
}