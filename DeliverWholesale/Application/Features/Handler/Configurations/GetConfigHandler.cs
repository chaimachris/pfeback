using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeliverWholesale.Domain.Entities;

namespace DeliverWholesale.Application.Features.Handler.Configurations
{
    public class GetConfigQuery : IRequest<Config> { }

    public class GetConfigHandler : IRequestHandler<GetConfigQuery, Config>
    {
        private readonly ApplicationDbContext _context;

        public GetConfigHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Config> Handle(GetConfigQuery request, CancellationToken cancellationToken)
        {
            var config = await _context.Configs.FirstOrDefaultAsync(cancellationToken);

            if (config == null)
            {
                config = new Config(); 
                _context.Configs.Add(config);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return config;
        }
    }
}