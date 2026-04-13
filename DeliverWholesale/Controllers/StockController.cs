using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Stock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddStock(AddStockLotDto dto)
        {
            var result = await _mediator.Send(new AddStockCommand(dto));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStock()
        {
            var stock = await _mediator.Send(new GetStockQuery());
            return Ok(stock);
        }

        [HttpGet("{produitId}")]
        public async Task<IActionResult> GetProductStock(int produitId)
        {
            var stock = await _mediator.Send(new GetProductStockQuery(produitId));
            return Ok(stock);
        }
    }
}