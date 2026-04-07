using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Dashboard
{
    public class GetDashboardAlertesQuery : IRequest<object> { }

    public class GetDashboardAlertesHandler : IRequestHandler<GetDashboardAlertesQuery, object>
    {
        private readonly ApplicationDbContext _context;

        public GetDashboardAlertesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GetDashboardAlertesQuery request, CancellationToken cancellationToken)
        {
            var config = await _context.Configs.FirstOrDefaultAsync();
            int seuil = config?.SeuilAlerteStockBas ?? 10;

            return await _context.Produits
                .Where(p => p.IsActive && p.StockActuel <= seuil)
                .Select(p => new
                {
                    Produit = p.Nom,
                    Stock = p.StockActuel,
                    EstCritique = p.StockActuel < 0
                })
                .ToListAsync();
        }
    }
}