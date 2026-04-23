using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.AchatLots
{
    public class DeleteAchatLotHandler : IRequestHandler<DeleteAchatLotCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteAchatLotHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAchatLotCommand request, CancellationToken cancellationToken)
        {
            var achat = await _context.AchatLots
                .Include(a => a.StockLots)
                .FirstOrDefaultAsync(a => a.Id == request.Id);

            if (achat == null)
                return false;

           
            _context.StockLots.RemoveRange(achat.StockLots);

          
            _context.AchatLots.Remove(achat);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}