using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Dashboard
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
            var config = await _context.Configs.FirstOrDefaultAsync(cancellationToken);
            int seuil = config?.SeuilAlerteStockBas ?? 10;

            var result = await _context.Produits
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    Produit = p.Nom,

                    
                    Stock = _context.StockLots
                        .Where(l => l.AchatLot.ProduitId == p.Id)
                        .Sum(l => l.QuantiteRestante),

                    EstCritique = _context.StockLots
                        .Where(l => l.AchatLot.ProduitId == p.Id)
                        .Sum(l => l.QuantiteRestante) < seuil
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}