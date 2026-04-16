using DeliverWholesale.Data;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Dashboard
{
    public class GetDashboardStatsQuery : IRequest<object> { }

    public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, object>
    {
        private readonly ApplicationDbContext _context;

        public GetDashboardStatsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var chiffre = await _context.Orders
                .Where(o => o.DateCommande >= DateTime.UtcNow.AddMonths(-1)
                         && o.Statut == StatutOrder.livree)
                .SumAsync(o => (decimal?)o.TotalFinal) ?? 0;

            return new
            {
                TotalProduits = await _context.Produits.CountAsync(p => p.IsActive),
                TotalCommandes = await _context.Orders.CountAsync(),
                CommandesEnAttente = await _context.Orders.CountAsync(o => o.Statut == StatutOrder.EnAttente),
                StockBas = await _context.Produits.CountAsync(p => p.StockActuel <= 10 && p.IsActive),
                StockNegatif = await _context.Produits.CountAsync(p => p.StockActuel < 0 && p.IsActive),
                ChiffreAffairesMois = chiffre
            };
        }
    }
}