using System.ComponentModel.DataAnnotations;

namespace Domain.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(150)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public string Role { get; set; }

        public ICollection<Job> JobsCreated { get; set; }
        public ICollection<Application> Applications { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

    }
}
