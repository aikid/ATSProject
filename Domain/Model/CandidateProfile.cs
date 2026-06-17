using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public class CandidateProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? LinkedIn { get; set; }

        [MaxLength(200)]
        public string? GitHub { get; set; }

        public string? Summary { get; set; }

        public string? Experience { get; set; }

        public string? Education { get; set; }

        public string? Skills { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
