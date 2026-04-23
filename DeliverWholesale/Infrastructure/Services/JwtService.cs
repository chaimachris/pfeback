using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliverWholesale.Infrastructure.Services
{
    public class JwtService
    {
        private readonly JwtConfig _config;
        private readonly SymmetricSecurityKey _key;

        public JwtService(IOptions<JwtConfig> config)
        {
            _config = config.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.ExpiryMinutes),
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}