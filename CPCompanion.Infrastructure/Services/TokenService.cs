using CPCompanion.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace CPCompanion.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }
        public string GenerateToken(string userId, string email)
        {

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };


            var secKey = _jwtSettings.SecretKey;

            if (string.IsNullOrEmpty(secKey))
                throw new InvalidOperationException("Secret Key Is Missing");

            if (secKey.Length < 32)
                throw new InvalidOperationException("Secret Key Is Too Short");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials 
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpiryMinutes { get; set; }
    }
}
