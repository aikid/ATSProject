using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Domain.Model;

namespace Api.Helpers
{
    public static class JwtHelper
    {
        private const string KeyId = "ats-rsa-key-1";
        //public static string GenerateToken(User user, IConfiguration config)
        //{
        //    var privateKey = File.ReadAllText(config["Jwt:PrivateKeyPath"]);
        //    var rsa = RSA.Create();
        //    rsa.ImportFromPem(privateKey.ToCharArray());

        //    var rsaKey = new RsaSecurityKey(rsa)
        //    {
        //        KeyId = "ats-rsa-key-1"
        //    };

        //    var credentials = new SigningCredentials(
        //        rsaKey,
        //        SecurityAlgorithms.RsaSha256
        //    );

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Email, user.Email)
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: config["Jwt:Issuer"],
        //        audience: config["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(
        //            int.Parse(config["Jwt:AccessTokenMinutes"])
        //        ),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        public static string GenerateToken(
            User user,
            IConfiguration config,
            RsaSecurityKey signingKey
        )
        {
            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.RsaSha256
            );

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(config["Jwt:AccessTokenMinutes"])
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
