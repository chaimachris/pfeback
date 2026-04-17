using DeliverWholesale.Data;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Services
{
    public class StockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetStock(int produitId)
        {
            var product = await _context.Produits.Include(p => p.StockLots).ThenInclude(l => l.Transactions).FirstOrDefaultAsync(p => p.Id == produitId);
            return product?.StockDisponible ?? 0;
        }
    }
}