using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class CandidaturasController : Controller
    {
        private readonly IApiClient _apiClient;

        public CandidaturasController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? status)
        {
            var caminho = "api/applications";
            if (!string.IsNullOrWhiteSpace(status))
                caminho += $"?status={Uri.EscapeDataString(status)}";

            var resultado = await _apiClient.GetAsync<List<MyApplicationDTO>>(caminho);

            ViewData["ActivePage"] = "MinhasCandidaturas";
            ViewData["StatusFilter"] = status;

            return View("Index", resultado.Sucesso ? resultado.Dado : new List<MyApplicationDTO>());
        }
    }
}
