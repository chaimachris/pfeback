using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Auth
{
    public class RegisterCommand : IRequest<string>
    {
        public RegisterDto Dto { get; set; }

        public RegisterCommand(RegisterDto dto)
        {
            Dto = dto;
        }
    }

    public class RegisterHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly ApplicationDbContext _context;

        public RegisterHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(x => x.Email == request.Dto.Email))
                throw new Exception("Email existe déjà");

            var names = request.Dto.FullName.Split(' ');

            var user = new User
            {
                Prenom = names[0],
                Nom = names.Length > 1 ? names[1] : "",
                Email = request.Dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password),
                Role = Role.Client
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Compte créé";
        }
    }
}