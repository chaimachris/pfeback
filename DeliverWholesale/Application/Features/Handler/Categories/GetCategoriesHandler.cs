using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Categories
{
    public class GetCategoriesQuery : IRequest<List<Categorie>> { }

    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<Categorie>>
    {
        private readonly ApplicationDbContext _context;

        public GetCategoriesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categorie>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Include(c => c.Produits)
                .ToListAsync();
        }
    }
}