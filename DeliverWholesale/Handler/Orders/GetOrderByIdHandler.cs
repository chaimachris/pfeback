using DeliverWholesale.Data;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Orders
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public int Id { get; set; }

        public GetOrderByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly ApplicationDbContext _context;

        public GetOrderByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Produit)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        }
    }
}