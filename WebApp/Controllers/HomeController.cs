using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class HomeController: Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            if (!AutenticacaoUtil.Autenticado(_httpContextAccessor))
                return RedirectToAction("Login", "Autenticacao");

            if (!AutenticacaoUtil.Autorizado(_httpContextAccessor, "esp_especialista"))
                return RedirectToAction("Dashboard");

            else return RedirectToAction("DashboardEspecialista");
        }
    }
}
