using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class VagasController : Controller
    {
        private readonly IApiClient _apiClient;

        public VagasController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] bool? active)
        {
            var filtro = active.HasValue ? $"?active={active.Value}" : "?active=true";

            if (!string.IsNullOrWhiteSpace(search))
                filtro += $"&search={Uri.EscapeDataString(search)}";

            var resultado = await _apiClient.GetAsync<List<JobResponseDTO>>($"api/jobs{filtro}");

            if (!resultado.Sucesso)
                return View("Index", new List<JobResponseDTO>());

            ViewData["ActivePage"] = "Vagas";
            ViewData["Search"] = search;
            ViewData["ActiveFilter"] = active;
            return View("Index", resultado.Dado);
        }

        [HttpGet]
        public IActionResult Nova()
        {
            ViewData["ActivePage"] = "Vagas";
            return View("Nova");
        }

        [HttpPost]
        public async Task<IActionResult> Nova(string Title, string Description, string Company, string Location)
        {
            var resultado = await _apiClient.PostAsync<JobRequestDTO, JobResponseDTO>(
                "api/jobs",
                new JobRequestDTO
                {
                    Title = Title,
                    Description = Description,
                    Company = Company,
                    Location = Location
                });

            if (!resultado.Sucesso)
                return Json(new { success = false, msg = resultado.Erro?.Mensagem ?? "Não foi possível criar a vaga." });

            return Json(new { success = true, id = resultado.Dado?.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var resultado = await _apiClient.GetAsync<JobResponseDTO>($"api/jobs/{id}");

            if (!resultado.Sucesso)
                return RedirectToAction("Index");

            ViewData["ActivePage"] = "Vagas";
            return View("Editar", resultado.Dado);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, string Title, string Description, string Company, string Location)
        {
            var resultado = await _apiClient.PutAsync<JobRequestDTO, JobResponseDTO>(
                $"api/jobs/{id}",
                new JobRequestDTO
                {
                    Title = Title,
                    Description = Description,
                    Company = Company,
                    Location = Location
                });

            if (!resultado.Sucesso)
                return Json(new { success = false, msg = resultado.Erro?.Mensagem ?? "Não foi possível atualizar a vaga." });

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var resultado = await _apiClient.PatchAsync<object>($"api/jobs/{id}/toggle");

            if (!resultado.Sucesso)
                return Json(new { success = false, msg = resultado.Erro?.Mensagem ?? "Não foi possível alterar o status." });

            return Json(new { success = true });
        }
    }
}
