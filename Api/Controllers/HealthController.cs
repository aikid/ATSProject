using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly AtsDbContext _db;

        public HealthController(AtsDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                _db.Database.ExecuteSqlRaw("SELECT 1");
                return Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy"
                });
            }
        }
    }
}
