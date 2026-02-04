using Domain.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApp.WebAppUtilities;

namespace WebApp.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private readonly IApiClient _apiClient;


        private static readonly string[] PublicRoutes =
        {
            "/",
            "/home",
            "/home/index",
            "/autenticacao",
            "/vagas",
        };

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger, IConfiguration configuration, IApiClient apiClient)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
            _apiClient = apiClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            if (path.StartsWith("/autenticacao") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/images"))
            {
                await _next(context);
                return;
            }

            if (PublicRoutes.Contains(path))
            {
                _logger.LogInformation(">>> Rota pública liberada");
                await _next(context);
                return;
            }

            var token = context.Request.Cookies["ACCESS_TOKEN"];
            _logger.LogInformation(">>> JWT: {Token}", token);
            _logger.LogInformation(">>> JWT presente? {HasToken}", !string.IsNullOrEmpty(token));

            if (string.IsNullOrEmpty(token))
            {
                context.Response.Redirect("/Autenticacao/Login");
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT expirado, tentando refresh...");

                var refreshToken = context.Request.Cookies["REFRESH_TOKEN"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshResult = await _apiClient.PostAsync<object, LoginResponseDTO>("api/autenticacao/refresh", new { RefreshToken = refreshToken });

                    if (refreshResult.Sucesso)
                    {
                        var isDev = context.Request.IsHttps == false;

                        context.Response.Cookies.Append("ACCESS_TOKEN", refreshResult.Dado.AccessToken, CookieUtil.AccessToken(isDev));
                        context.Response.Cookies.Append("REFRESH_TOKEN", refreshResult.Dado.RefreshToken, CookieUtil.RefreshToken(isDev));

                        _logger.LogInformation("Refresh realizado com sucesso");
                        await _next(context);
                        return;
                    }
                }

                _logger.LogWarning("Refresh falhou, redirecionando para login");
                context.Response.Redirect("/Autenticacao/Login");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("JWT inválido: {Message}", ex.Message);
                context.Response.Redirect("/Autenticacao/Login");
            }

        }
    }

}
