using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApp.WebAppUtilities
{
    public static class AutenticacaoUtil
    {
        private static ClaimsPrincipal ObterPrincipal(IHttpContextAccessor accessor)
        {
            var token = accessor.HttpContext?.Request.Cookies["ACCESS_TOKEN"];
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(jwt.Claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return null;
            }
        }

        // 🔐 Usuário está autenticado?
        public static bool Autenticado(IHttpContextAccessor accessor)
        {
            var principal = ObterPrincipal(accessor);
            return principal?.Identity?.IsAuthenticated == true;
        }

        // 👤 Id do usuário
        public static string ObterId(IHttpContextAccessor accessor)
        {
            return ObterPrincipal(accessor)?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? "sessão expirou";
        }

        // 📧 Email
        public static string ObterEmail(IHttpContextAccessor accessor)
        {
            return ObterPrincipal(accessor)?
                .FindFirst(ClaimTypes.Email)?.Value
                ?? "sessão expirou";
        }

        // 🧑 Nome (se existir no token)
        public static string ObterNome(IHttpContextAccessor accessor)
        {
            return ObterPrincipal(accessor)?
                .FindFirst(ClaimTypes.Name)?.Value
                ?? "sessão expirou";
        }

        // 🛡️ Perfil / Role
        public static bool Autorizado(IHttpContextAccessor accessor, string roles)
        {
            var principal = ObterPrincipal(accessor);
            if (principal == null) return false;

            return roles
                .Split(',')
                .Any(role => principal.IsInRole(role.Trim()));
        }

        // 🔑 Rules / Permissions customizadas
        public static bool AutorizadoRules(IHttpContextAccessor accessor, string rule)
        {
            var principal = ObterPrincipal(accessor);
            if (principal == null) return false;

            return principal.Claims.Any(c =>
                c.Type == "permission" && c.Value == rule);
        }
    }
}
