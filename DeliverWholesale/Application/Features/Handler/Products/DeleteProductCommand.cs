using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    // ✅ COMMAND
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteProductCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteProductHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Produits
                .FirstOrDefaultAsync(p => p.idP == request.Id, cancellationToken);

            if (product == null)
                return false;

            product.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}