using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class GetAllAchatLotsHandler : IRequestHandler<GetAllAchatLotsQuery, List<AchatLot>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllAchatLotsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AchatLot>> Handle(GetAllAchatLotsQuery request, CancellationToken cancellationToken)
        {
            return await _context.AchatLots
                .Include(a => a.Produit)
                .Include(a => a.StockLots)
                .ToListAsync(cancellationToken);
        }
    }
}