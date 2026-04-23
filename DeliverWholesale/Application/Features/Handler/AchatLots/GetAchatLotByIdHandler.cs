using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class GetAchatLotByIdHandler : IRequestHandler<GetAchatLotByIdQuery, AchatLot>
    {
        private readonly ApplicationDbContext _context;

        public GetAchatLotByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AchatLot> Handle(GetAchatLotByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.AchatLots
                .Include(a => a.Produit)
                .Include(a => a.StockLots)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        }
    }
}