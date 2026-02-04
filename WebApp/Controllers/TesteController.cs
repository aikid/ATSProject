using Microsoft.AspNetCore.Mvc;
using WebApp.WebAppUtilities;

namespace WebApp.Controllers
{
    public class TesteController : Controller
    {
        private readonly IApiClient _apiClient;

        public TesteController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> ChamadaProtegida()
        {
            var resultado = await _apiClient.GetAsync<string>("api/teste/protegido");

            if (!resultado.Sucesso)
                return Content("❌ Falhou: " + resultado.Erro?.Mensagem);

            return Content("✅ Sucesso após refresh automático!");
        }
    }
}
