using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class AutenticacaoController : Controller
    {
        private IConfiguration _config { get; }
        private string _apiATSPath { get; }

        private readonly IWebHostEnvironment _webhostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AutenticacaoController(IConfiguration configuration, IWebHostEnvironment webhostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration;
            _apiATSPath = _config.GetSection("WebAppUtil:apiATSPath").Value;
            _webhostEnvironment = webhostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string USUARIO, string SENHA)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_apiATSPath);

            var response = await client.PostAsJsonAsync("/api/autenticacao/login", new
            {
                USUARIO,
                SENHA
            });

            if (!response.IsSuccessStatusCode)
                return Json(new { req = false, msg = "Usuário ou senha inválidos." });

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

            var isDev = _httpContextAccessor.HttpContext.Request.IsHttps == false;

            Response.Cookies.Append("ACCESS_TOKEN", result.AccessToken, CookieUtil.AccessToken(isDev));

            Response.Cookies.Append("REFRESH_TOKEN", result.RefreshToken, CookieUtil.RefreshToken(isDev));

            return Json(new {
                success = true,
            });
        }


        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(_apiATSPath)
                };

                await client.PostAsJsonAsync("/api/autenticacao/logout", refreshToken);
            }

            Response.Cookies.Delete("ACCESS_TOKEN", CookieUtil.Logout());

            Response.Cookies.Delete("REFRESH_TOKEN", CookieUtil.Logout());

            return RedirectToAction("Login", "Autenticacao");
        }

    }
}
