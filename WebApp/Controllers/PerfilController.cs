using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IApiClient _apiClient;

        public PerfilController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var resultado = await _apiClient.GetAsync<MyProfileDTO>("api/profile");

            ViewData["ActivePage"] = "Perfil";

            if (!resultado.Sucesso)
                return View("Index", new MyProfileDTO());

            return View("Index", resultado.Dado);
        }

        [HttpPost]
        public async Task<IActionResult> Salvar(
            string? FullName, string? Phone,
            string ProfileFullName, string? ProfilePhone,
            string? LinkedIn, string? GitHub,
            string? Summary, string? Skills,
            string? Experience, string? Education)
        {
            var resultado = await _apiClient.PutAsync<MyProfileDTO, object>(
                "api/profile",
                new MyProfileDTO
                {
                    FullName = FullName,
                    Phone = Phone,
                    ProfileFullName = ProfileFullName ?? string.Empty,
                    ProfilePhone = ProfilePhone,
                    LinkedIn = LinkedIn,
                    GitHub = GitHub,
                    Summary = Summary,
                    Skills = Skills,
                    Experience = Experience,
                    Education = Education
                });

            if (!resultado.Sucesso)
                return Json(new { success = false, msg = resultado.Erro?.Mensagem ?? "Não foi possível salvar o perfil." });

            return Json(new { success = true });
        }
    }
}
