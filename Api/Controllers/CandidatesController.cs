using Api.Data;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Recruiter")]
    public class CandidatesController : ControllerBase
    {
        private readonly AtsDbContext _db;

        public CandidatesController(AtsDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] bool? active)
        {
            var query = _db.Users
                .Where(u => u.Role == "Candidate")
                .AsQueryable();

            if (active.HasValue)
                query = query.Where(u => u.IsActive == active.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.Contains(search)) ||
                    u.Email.Contains(search));

            var candidates = await query
                .OrderBy(u => u.FullName ?? u.Email)
                .Select(u => new CandidateListItemDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    HasProfile = u.CandidateProfile != null,
                    ApplicationCount = u.Applications.Count
                })
                .ToListAsync();

            return Ok(candidates);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _db.Users
                .Include(u => u.CandidateProfile)
                .Include(u => u.Applications)
                    .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "Candidate");

            if (user is null)
                return NotFound();

            var dto = new CandidateDetailDTO
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                IsActive = user.IsActive,
                ProfileFullName = user.CandidateProfile?.FullName,
                LinkedIn = user.CandidateProfile?.LinkedIn,
                GitHub = user.CandidateProfile?.GitHub,
                Summary = user.CandidateProfile?.Summary,
                Skills = user.CandidateProfile?.Skills,
                Experience = user.CandidateProfile?.Experience,
                Education = user.CandidateProfile?.Education,
                ProfileCreatedAt = user.CandidateProfile?.CreatedAt,
                Applications = user.Applications
                    .OrderByDescending(a => a.AppliedAt)
                    .Select(a => new CandidateApplicationDTO
                    {
                        Id = a.Id,
                        JobTitle = a.Job.Title,
                        Company = a.Job.Company,
                        Status = a.Status,
                        AppliedAt = a.AppliedAt
                    })
                    .ToList()
            };

            return Ok(dto);
        }

        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user is null || user.Role != "Candidate")
                return NotFound();

            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();

            return Ok(new { isActive = user.IsActive });
        }
    }
}
