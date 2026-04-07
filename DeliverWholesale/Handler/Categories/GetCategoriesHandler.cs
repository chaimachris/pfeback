using DeliverWholesale.Data;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Categories
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