namespace WebApp.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly string[] PublicRoutes =
        {
            "/",
            "/home",
            "/home/index",
            "/autenticacao",
            "/vagas",
        };

        public AuthMiddleware(RequestDelegate next)
        {
            Console.WriteLine(">>> AuthMiddleware CONSTRUIDO");
            _next = next;
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

            if (PublicRoutes.Any(r => path == r || path.StartsWith(r + "/")))
            {
                await _next(context);
                return;
            }

            var token = context.Session.GetString("JWT");

            Console.WriteLine($"[AuthMiddleware] Path: {path} | JWT: {(token != null)}");

            if (string.IsNullOrEmpty(token))
            {
                context.Response.Redirect("/Autenticacao/Login");
                return;
            }

            await _next(context);
        }
    }

}
