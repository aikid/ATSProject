using Domain.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

            var accessToken = context.Request.Cookies["ACCESS_TOKEN"];

            // Token presente e válido — popula o usuário e segue
            if (!string.IsNullOrEmpty(accessToken) && TrySetUser(context, accessToken))
            {
                await _next(context);
                return;
            }

            // Token expirado ou ausente — tenta refresh
            _logger.LogWarning("Usuário não autenticado, tentando refresh...");

            var novoToken = await TentarRefreshAsync(context);

            if (novoToken is not null)
            {
                _logger.LogInformation("Refresh realizado com sucesso, revalidando token...");

                // Popula o usuário com o token recém-obtido
                if (TrySetUser(context, novoToken))
                {
                    await _next(context);
                    return;
                }
            }

            _logger.LogWarning("Refresh falhou, redirecionando para login");
            context.Response.Redirect("/Autenticacao/Login");
        }

        /// <summary>
        /// Valida o JWT, popula context.User e context.Items.
        /// Retorna true se o token for válido.
        /// </summary>
        private bool TrySetUser(HttpContext context, string token)
        {
            try
            {
                var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

                var publicKeyPath = Path.Combine(
                    env.ContentRootPath,
                    _configuration["Jwt:PublicKeyPath"]
                );

                var publicKey = File.ReadAllText(publicKeyPath);

                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKey.ToCharArray());

                var validationKey = new RsaSecurityKey(rsa) { KeyId = "ats-rsa-key-1" };

                var principal = new JwtSecurityTokenHandler().ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = validationKey,
                        ClockSkew = TimeSpan.Zero
                    },
                    out var validatedToken
                );

                var jwt = (JwtSecurityToken)validatedToken;

                context.Items["ACCESS_TOKEN"] = token;
                context.Items["UserId"] = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                context.Items["Email"] = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                context.Items["Role"] = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                context.User = principal;

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT expirado.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("JWT inválido: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Tenta renovar os tokens via refresh. Retorna o novo ACCESS_TOKEN ou null em caso de falha.
        /// </summary>
        private async Task<string?> TentarRefreshAsync(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["REFRESH_TOKEN"];

            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var response = await _apiClient.PostAsync<RefreshRequestDTO, RefreshResponseDTO>(
                "api/autenticacao/refresh",
                new RefreshRequestDTO { RefreshToken = refreshToken }
            );

            if (!response.Sucesso)
                return null;

            var isDev = context.Request.IsHttps == false;

            context.Response.Cookies.Append("ACCESS_TOKEN", response.Dado.AccessToken, CookieUtil.AccessToken(isDev));
            context.Response.Cookies.Append("REFRESH_TOKEN", response.Dado.RefreshToken, CookieUtil.RefreshToken(isDev));

            return response.Dado.AccessToken;
        }
    }
}

