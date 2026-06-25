using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly IApiClient _apiClient;

        public CandidatosController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] bool? active)
        {
            var filtro = active.HasValue ? $"?active={active.Value}" : "?active=true";

            if (!string.IsNullOrWhiteSpace(search))
                filtro += $"&search={Uri.EscapeDataString(search)}";

            var resultado = await _apiClient.GetAsync<List<CandidateListItemDTO>>($"api/candidates{filtro}");

            ViewData["ActivePage"] = "Candidatos";
            ViewData["Search"] = search;
            ViewData["ActiveFilter"] = active;

            return View("Index", resultado.Sucesso ? resultado.Dado : new List<CandidateListItemDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var resultado = await _apiClient.GetAsync<CandidateDetailDTO>($"api/candidates/{id}");

            if (!resultado.Sucesso)
                return RedirectToAction("Index");

            ViewData["ActivePage"] = "Candidatos";
            return View("Detalhes", resultado.Dado);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var resultado = await _apiClient.PatchAsync<object>($"api/candidates/{id}/toggle");

            if (!resultado.Sucesso)
                return Json(new { success = false, msg = resultado.Erro?.Mensagem ?? "Não foi possível alterar o status." });

            return Json(new { success = true });
        }
    }
}
