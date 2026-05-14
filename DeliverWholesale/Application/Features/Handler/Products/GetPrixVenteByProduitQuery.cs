using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;           // ← AJOUTÉ

using MediatR;

namespace DeliverWholesale.Application.Features.Handler.Products
{
    public class GetPrixVenteByProduitQuery : IRequest<List<PrixVente>>
    {
        public int idP { get; set; }

        public GetPrixVenteByProduitQuery(int idP)
        {
            this.idP = idP;
        }
    }
    public class GetPrixVenteByProduitHandler : IRequestHandler<GetPrixVenteByProduitQuery, List<PrixVente>>
    {
        private readonly ApplicationDbContext _context;

        public GetPrixVenteByProduitHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PrixVente>> Handle(GetPrixVenteByProduitQuery request, CancellationToken cancellationToken)
        {
            return await _context.PrixVentes
                .Where(p => p.idP == request.idP)
                .OrderByDescending(p => p.Date)
                .ToListAsync(cancellationToken);
        }
    }
}
