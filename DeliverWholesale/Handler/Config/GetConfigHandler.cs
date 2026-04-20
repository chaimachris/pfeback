using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Config
{
    public class GetConfigQuery : IRequest<Models.Config> { }

    public class GetConfigHandler : IRequestHandler<GetConfigQuery, Models.Config>
    {
        private readonly ApplicationDbContext _context;

        public GetConfigHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Config> Handle(GetConfigQuery request, CancellationToken cancellationToken)
        {
            var config = await _context.Configs.FirstOrDefaultAsync();

            if (config == null)
            {
                config = new Models.Config();
                _context.Configs.Add(config);
                await _context.SaveChangesAsync();
            }

            return config;
        }
    }
}