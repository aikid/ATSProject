using Api.Data;
using Api.Helpers;
using Api.Security;
using Domain.DTOs;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AtsDbContext _db;
        private readonly RsaSecurityKey _signingKey;

        public AutenticacaoController(IConfiguration configuration, AtsDbContext db, RsaSecurityKey signingKey)
        {
            _configuration = configuration;
            _db = db;
            _signingKey = signingKey;
        }

        [IgnoreApiKey]
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDTO dto)
        {
            Console.WriteLine(">>> CHEGOU NO LOGIN DA API");
            var user = _db.Users.FirstOrDefault(u => u.Email == dto.USUARIO);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.SENHA, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos");

            var jwt = JwtHelper.GenerateToken(user, _configuration, _signingKey);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _db.RefreshTokens.Add(refreshToken);
            _db.SaveChanges();

            return Ok(new LoginResponseDTO
            {
                AccessToken = jwt,
                RefreshToken = refreshToken.Token
            });
        }

        [IgnoreApiKey]
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequestDTO dto)
        {
            var refresh = _db.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefault(r =>
                    r.Token == dto.RefreshToken &&
                    !r.IsRevoked &&
                    r.ExpiresAt > DateTime.UtcNow);

            if (refresh == null)
                return Unauthorized();

            // revoga o antigo
            refresh.IsRevoked = true;

            // gera novo refresh
            var newRefresh = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = refresh.UserId
            };

            _db.RefreshTokens.Add(newRefresh);

            var accessToken = JwtHelper.GenerateToken(refresh.User, _configuration, _signingKey);

            _db.SaveChanges();

            return Ok(new RefreshResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh.Token
            });
        }

        [IgnoreApiKey]
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            var token = _db.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (token == null)
                return Ok();

            token.IsRevoked = true;
            _db.SaveChanges();

            return Ok();
        }


    }
}
