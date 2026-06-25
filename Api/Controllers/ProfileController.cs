using Api.Data;
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
    [Authorize(Roles = "Candidate")]
    public class ProfileController : ControllerBase
    {
        private readonly AtsDbContext _db;

        public ProfileController(AtsDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var user = await _db.Users
                .Include(u => u.CandidateProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) return NotFound();

            return Ok(new MyProfileDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                HasProfile = user.CandidateProfile != null,
                ProfileCreatedAt = user.CandidateProfile?.CreatedAt,
                ProfileFullName = user.CandidateProfile?.FullName ?? user.FullName ?? string.Empty,
                ProfilePhone = user.CandidateProfile?.Phone ?? user.Phone,
                LinkedIn = user.CandidateProfile?.LinkedIn,
                GitHub = user.CandidateProfile?.GitHub,
                Summary = user.CandidateProfile?.Summary,
                Skills = user.CandidateProfile?.Skills,
                Experience = user.CandidateProfile?.Experience,
                Education = user.CandidateProfile?.Education
            });
        }

        [HttpPut]
        public async Task<IActionResult> Save([FromBody] MyProfileDTO dto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var user = await _db.Users
                .Include(u => u.CandidateProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) return NotFound();

            // Atualiza dados da conta
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;

            // Cria ou atualiza o perfil profissional
            if (user.CandidateProfile is null)
            {
                user.CandidateProfile = new CandidateProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.CandidateProfiles.Add(user.CandidateProfile);
            }

            user.CandidateProfile.FullName = dto.ProfileFullName;
            user.CandidateProfile.Phone = dto.ProfilePhone;
            user.CandidateProfile.LinkedIn = dto.LinkedIn;
            user.CandidateProfile.GitHub = dto.GitHub;
            user.CandidateProfile.Summary = dto.Summary;
            user.CandidateProfile.Skills = dto.Skills;
            user.CandidateProfile.Experience = dto.Experience;
            user.CandidateProfile.Education = dto.Education;

            await _db.SaveChangesAsync();

            return Ok(new { mensagem = "Perfil atualizado com sucesso." });
        }
    }
}
