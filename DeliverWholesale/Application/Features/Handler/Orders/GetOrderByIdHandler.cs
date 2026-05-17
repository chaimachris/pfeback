using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetOrderByIdHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new ApplicationException("Utilisateur non authentifié.");

            var userId = int.Parse(userIdClaim);

            return await _context.Orders
                .Where(o => o.Id == request.Id && o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Produit)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
