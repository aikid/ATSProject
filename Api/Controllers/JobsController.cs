using Api.Data;
using Api.Security;
using Domain.DTOs;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly AtsDbContext _db;

        public JobsController(AtsDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? active, [FromQuery] string? search)
        {
            var query = _db.Jobs
                .Include(j => j.CreatedBy)
                .AsQueryable();

            if (active.HasValue)
                query = query.Where(j => j.IsActive == active.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.Company.Contains(search) ||
                    j.Location.Contains(search));

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobResponseDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Company = j.Company,
                    Location = j.Location,
                    IsActive = j.IsActive,
                    CreatedAt = j.CreatedAt,
                    CreatedByUserId = j.CreatedByUserId,
                    CreatedByName = j.CreatedBy.FullName ?? j.CreatedBy.Email
                })
                .ToListAsync();

            return Ok(jobs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _db.Jobs
                .Include(j => j.CreatedBy)
                .Where(j => j.Id == id)
                .Select(j => new JobResponseDTO
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Company = j.Company,
                    Location = j.Location,
                    IsActive = j.IsActive,
                    CreatedAt = j.CreatedAt,
                    CreatedByUserId = j.CreatedByUserId,
                    CreatedByName = j.CreatedBy.FullName ?? j.CreatedBy.Email
                })
                .FirstOrDefaultAsync();

            if (job is null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> Create([FromBody] JobRequestDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Company = dto.Company,
                Location = dto.Location,
                IsActive = true,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = job.Id }, new JobResponseDTO
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Company = job.Company,
                Location = job.Location,
                IsActive = job.IsActive,
                CreatedAt = job.CreatedAt,
                CreatedByUserId = job.CreatedByUserId
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> Update(int id, [FromBody] JobRequestDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var job = await _db.Jobs.FindAsync(id);

            if (job is null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && job.CreatedByUserId != userId)
                return Forbid();

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Company = dto.Company;
            job.Location = dto.Location;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/toggle")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> Toggle(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var job = await _db.Jobs.FindAsync(id);

            if (job is null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && job.CreatedByUserId != userId)
                return Forbid();

            job.IsActive = !job.IsActive;
            await _db.SaveChangesAsync();

            return Ok(new { isActive = job.IsActive });
        }
    }
}
