namespace WebApp.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        private static readonly string[] PublicRoutes =
        {
            "/",
            "/home",
            "/home/index",
            "/autenticacao",
            "/vagas",
        };

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _logger.LogInformation(">>> AuthMiddleware CONSTRUIDO");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            _logger.LogInformation(">>> AuthMiddleware Invoke | Path: {Path}", path);

            if (path.StartsWith("/autenticacao") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/images"))
            {
                await _next(context);
                return;
            }

            if (PublicRoutes.Any(r => path == r || path.StartsWith(r + "/")))
            {
                _logger.LogInformation(">>> Rota pública liberada");
                await _next(context);
                return;
            }

            var token = context.Session.GetString("JWT");

            _logger.LogInformation(">>> JWT presente? {HasToken}", !string.IsNullOrEmpty(token));

            Console.WriteLine($"[AuthMiddleware] Path: {path} | JWT: {(token != null)}");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning(">>> SEM JWT → redirect login");
                context.Response.Redirect("/Autenticacao/Login");
                return;
            }

            await _next(context);
        }
    }

}
