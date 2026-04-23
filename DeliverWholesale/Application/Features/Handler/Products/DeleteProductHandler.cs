using DeliverWholesale.Infrastructure.Data;
using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Products
{
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
            var product = await _context.Produits.FindAsync(request.Id);

            if (product == null)
                return false;

            _context.Produits.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}