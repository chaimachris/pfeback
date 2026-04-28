using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliverWholesale.Infrastructure.Services
{
    public class JwtService
    {
        private readonly JwtConfig _config;
        private readonly SymmetricSecurityKey _key;

        
        private static readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

        public JwtService(IOptions<JwtConfig> config)
        {
            _config = config.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        }

        // ========================
        // GÉNÉRATION DU TOKEN
        // ========================
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

        
        public void RevokeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var expiry = GetTokenExpiry(token);
            _revokedTokens[token] = expiry ?? DateTime.UtcNow.AddMinutes(_config.ExpiryMinutes);

            // Nettoyage des tokens expirés pour éviter la fuite mémoire
            CleanupExpiredTokens();
        }

     
        public bool IsTokenRevoked(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            return _revokedTokens.ContainsKey(token);
        }

        // ========================
        // UTILITAIRES PRIVÉS
        // ========================

        
        private DateTime? GetTokenExpiry(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo;
            }
            catch
            {
                return null;
            }
        }

        
        private static void CleanupExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _revokedTokens
                .Where(kvp => kvp.Value < now)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
                _revokedTokens.TryRemove(key, out _);
        }
    }
}