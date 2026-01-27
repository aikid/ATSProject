using Microsoft.AspNetCore.Mvc;
using WebApp.Models.DTOs;

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

            // 🔐 Salvar JWT (Session por enquanto)
            _httpContextAccessor.HttpContext.Session.SetString("JWT", result.accessToken);

            return Json(new { req = true, cond = false });
        }


        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Login", "Autenticacao");
        }
    }
}
