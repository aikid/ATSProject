using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Domain.Model;

namespace Api.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(
            User user,
            IConfiguration config,
            RsaSecurityKey signingKey
        )
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("user_id", user.Id.ToString()),
                new Claim("is_active", user.IsActive.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(config["Jwt:AccessTokenMinutes"])
                ),
                signingCredentials: new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.RsaSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
