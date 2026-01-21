namespace WebApp.WebAppUtilities
{
    public static class AutenticacaoUtil
    {
        public static bool Autenticado(IHttpContextAccessor _httpContextAccessor)
        {
            var autenticado = _httpContextAccessor.HttpContext.Session.GetString("autorizado");
            return !string.IsNullOrEmpty(autenticado) && Convert.ToBoolean(autenticado);
        }

        public static bool Autorizado(IHttpContextAccessor _httpContextAccessor, string auth)
        {
            var perfil = _httpContextAccessor.HttpContext.Session.GetString("nm_perfil");
            return !string.IsNullOrEmpty(perfil) && auth.Contains(perfil);
        }

        public static bool AutorizadoRules(IHttpContextAccessor _httpContextAccessor, string auth)
        {
            var rules = _httpContextAccessor.HttpContext.Session.GetString("rules");
            return !string.IsNullOrEmpty(rules) && rules.Contains(auth);
        }

        public static string ObterHashUser(IHttpContextAccessor _httpContextAccessor)
        {
            var hashUser = _httpContextAccessor.HttpContext.Session.GetString("hash_user");
            if (!string.IsNullOrEmpty(hashUser))
                return hashUser;

            return "sessão expirou";
        }

        public static string ObterId(IHttpContextAccessor _httpContextAccessor)
        {
            var id = _httpContextAccessor.HttpContext.Session.GetString("id_usuario_externo");
            if (!string.IsNullOrEmpty(id))
                return id;

            return "sessão expirou";
        }

        public static string ObterEmail(IHttpContextAccessor _httpContextAccessor)
        {
            var email = _httpContextAccessor.HttpContext.Session.GetString("email");
            if (!string.IsNullOrEmpty(email))
                return email;

            return "sessão expirou";
        }

        public static string ObterNome(IHttpContextAccessor _httpContextAccessor)
        {
            var nome = _httpContextAccessor.HttpContext.Session.GetString("nm_usuario");
            if (!string.IsNullOrEmpty(nome))
                return nome;

            else return "sessão expirou";
        }
    }
}