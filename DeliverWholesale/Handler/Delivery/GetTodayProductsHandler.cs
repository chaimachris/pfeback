using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Delivery
{
    public class GetTodayProductsHandler : IRequestHandler<GetTodayProductsQuery, object>
    {
        private readonly ApplicationDbContext _context;

        public GetTodayProductsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GetTodayProductsQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Produit)
                .Where(od => od.Order.DateCommande.Date == today)
                .Select(od => new
                {
                    Produit = od.Produit.Nom,
                    Quantite = od.Quantite,
                    Prix = od.PrixUnitaire,
                    OrderId = od.OrderId
                })
                .ToListAsync(cancellationToken);
        }
    }
}