using Api.Helpers;
using Domain.DTOs;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController: Controller
    {
        private readonly IConfiguration _configuration;
        public AutenticacaoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("login")]
        public IActionResult Login(LoginRequestDTO request)
        {
            // 1. Validar usuário (email/cpf + senha)
            // (mock por enquanto)
            if (request.USUARIO != "admin@admim.com" || request.SENHA != "123")
                return Unauthorized();


            var token = JwtHelper.GenerateToken(
                userId: 1,
                email: request.USUARIO,
                configuration: _configuration
            );

            return Ok(new
            {
                accessToken = token,
                expiresIn = 3600
            });
        }
    }
}
