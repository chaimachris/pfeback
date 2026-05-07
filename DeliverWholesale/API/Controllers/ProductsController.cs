using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Application.Features.Handler.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
        {
            var result = await _mediator.Send(new CreateProductCommand(dto));

            return Ok(result);
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetProductsQuery());

            return Ok(products);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] ProductUpdateDto dto)
        {
            var result = await _mediator.Send(
                new UpdateProductCommand(id, dto));

            if (!result)
                return NotFound();

            return Ok();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(
                new DeleteProductCommand(id));

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}