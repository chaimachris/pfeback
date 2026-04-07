using DeliverWholesale.Data;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeliverWholesale.Handler.Orders
{
    public class GetOrdersQuery : IRequest<List<Order>> { }

    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<Order>>
    {
        private readonly ApplicationDbContext _context;

        public GetOrdersHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Produit)
                .ToListAsync(cancellationToken);
        }
    }
}