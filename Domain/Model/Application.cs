using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; }

        [Required]
        public int CandidateUserId { get; set; }

        [ForeignKey(nameof(CandidateUserId))]
        public User Candidate { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Applied";
    }
}
