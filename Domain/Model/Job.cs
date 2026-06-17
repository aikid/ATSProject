using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Model
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [MaxLength(150)]
        public string Company { get; set; }

        [MaxLength(150)]
        public string Location { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Application> Applications { get; set; }
    }
}
