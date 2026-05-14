using DeliverWholesale.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Application.Features.Handler.Auth
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public ConfirmEmailHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.EmailConfirmationToken == request.Token);

            if (user == null)
                return false;

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}