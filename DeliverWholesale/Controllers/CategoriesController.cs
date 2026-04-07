using DeliverWholesale.DTOs;
using DeliverWholesale.Handler.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverWholesale.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var category = await _mediator.Send(new CreateCategoryCommand(dto));
            return Ok(category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            var success = await _mediator.Send(new UpdateCategoryCommand(id, dto));

            if (!success)
                return NotFound("Catégorie introuvable");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteCategoryCommand(id));

            if (!success)
                return NotFound("Catégorie introuvable");

            return NoContent();
        }
    }
}