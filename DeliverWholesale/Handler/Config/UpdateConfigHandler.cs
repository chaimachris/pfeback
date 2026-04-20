using DeliverWholesale.Data;
using MediatR;

namespace DeliverWholesale.Handler.Config
{
    public class UpdateConfigCommand : IRequest<bool>
    {
        public Models.Config UpdatedConfig { get; set; }

        public UpdateConfigCommand(Models.Config config)
        {
            UpdatedConfig = config;
        }
    }

    public class UpdateConfigHandler : IRequestHandler<UpdateConfigCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateConfigHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
        {
            var config = await _context.Configs.FindAsync(1);

            if (config == null)
                return false;

            config.MontantMinimumCommande = request.UpdatedConfig.MontantMinimumCommande;
            config.FraisLivraison = request.UpdatedConfig.FraisLivraison;
            config.ProfitPercentage = request.UpdatedConfig.ProfitPercentage;
            config.SeuilAlerteStockBas = request.UpdatedConfig.SeuilAlerteStockBas;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}