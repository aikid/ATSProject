using Api.Data;
using Api.Helpers;
using Domain.DTOs;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AtsDbContext _db;

        public AutenticacaoController(IConfiguration configuration, AtsDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDTO dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == dto.USUARIO);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.SENHA, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos");

            var jwt = JwtHelper.GenerateToken(user, _configuration);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            _db.SaveChanges();

            return Ok(new LoginResponseDTO
            {
                AccessToken = jwt,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            var token = _db.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token inválido");

            var user = _db.Users.Find(token.UserId);

            var newJwt = JwtHelper.GenerateToken(user, _configuration);
            var newRefreshToken = JwtHelper.GenerateRefreshToken();

            token.IsRevoked = true;

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            _db.SaveChanges();

            return Ok(new LoginResponseDTO
            {
                AccessToken = newJwt,
                RefreshToken = newRefreshToken
            });
        }

    }
}
