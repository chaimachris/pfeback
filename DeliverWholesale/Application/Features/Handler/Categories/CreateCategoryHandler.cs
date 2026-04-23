using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Categories
{
    public class CreateCategoryCommand : IRequest<Categorie>
    {
        public CategoryDto Dto { get; set; }

        public CreateCategoryCommand(CategoryDto dto)
        {
            Dto = dto;
        }
    }

    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Categorie>
    {
        private readonly ApplicationDbContext _context;

        public CreateCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Categorie> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Categorie
            {
                Nom = request.Dto.Nom,
                Description = request.Dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}