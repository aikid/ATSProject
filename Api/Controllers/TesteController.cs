using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        [Authorize]
        [HttpGet("protegido")]
        public IActionResult Protegido()
        {
            return Ok("Acesso autorizado");
        }
    }
}
