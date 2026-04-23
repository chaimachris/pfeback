using DeliverWholesale.Application.DTOs.DTOs;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Domain.Entities;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Categories
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public CategoryDto Dto { get; set; }

        public UpdateCategoryCommand(int id, CategoryDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.Id);

            if (category == null)
                return false;

            category.Nom = request.Dto.Nom;
            category.Description = request.Dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}