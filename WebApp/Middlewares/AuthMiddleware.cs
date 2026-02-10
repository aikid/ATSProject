using Domain.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
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

            // ✅ JWT JÁ FOI VALIDADO PELO JwtBearer
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            _logger.LogWarning("REFRESH COOKIE: {Value}", context.Request.Cookies["REFRESH_TOKEN"]);

            // 🔄 tenta refresh
            _logger.LogWarning("Usuário não autenticado, tentando refresh...");

            if (await TentarRefreshAsync(context))
            {
                _logger.LogInformation("Refresh realizado com sucesso");
                await _next(context);
                return;
            }

            _logger.LogWarning("Refresh falhou, redirecionando para login");
            context.Response.Redirect("/Autenticacao/Login");
        }
        private async Task<bool> TentarRefreshAsync(HttpContext context) {

            var refreshToken = context.Request.Cookies["REFRESH_TOKEN"];

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _apiClient.PostAsync<RefreshRequestDTO, RefreshResponseDTO>(
                "api/autenticacao/refresh",
                new RefreshRequestDTO { RefreshToken = refreshToken }
            );

            if (!response.Sucesso)
                return false;

            var isDev = context.Request.IsHttps == false;

            context.Response.Cookies.Append("ACCESS_TOKEN", response.Dado.AccessToken, CookieUtil.AccessToken(isDev));
            context.Response.Cookies.Append("REFRESH_TOKEN", response.Dado.RefreshToken, CookieUtil.RefreshToken(isDev));

            return true;
        }
    }

}
