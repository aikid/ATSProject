using Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        [ApiKey]
        [HttpGet("protegido")]
        public IActionResult Protegido()
        {
            return Ok("Acesso autorizado");
        }

        [Authorize]
        [HttpGet("naoprotegido")]
        public IActionResult NaoProtegido()
        {
            return Ok("Acesso autorizado");
        }
    }
}
