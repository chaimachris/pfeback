using DeliverWholesale.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Auth
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, string>
    {
        private readonly ApplicationDbContext _context;

        public ConfirmEmailHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.EmailConfirmationToken == request.Token);

            if (user == null)
                throw new Exception("Lien invalide");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;

            await _context.SaveChangesAsync();
            //0sw1woASAYKZJD69oS4yozKvKfijyhXW     <=== key
            //SKd1313d86816db4918b5a4e3ecf056e83 <=== sid
            return "Email confirmé avec succès";
        }
    }
}