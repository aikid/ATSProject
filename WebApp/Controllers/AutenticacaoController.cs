using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
