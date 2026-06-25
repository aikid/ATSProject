using Api.Data;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Candidate")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AtsDbContext _db;

        public ApplicationsController(AtsDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine([FromQuery] string? status)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var query = _db.Applications
                .Include(a => a.Job)
                .Where(a => a.CandidateUserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            var list = await query
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new MyApplicationDTO
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    Company = a.Job.Company,
                    Location = a.Job.Location,
                    JobDescription = a.Job.Description,
                    Status = a.Status,
                    AppliedAt = a.AppliedAt,
                    JobIsActive = a.Job.IsActive
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
