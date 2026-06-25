using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class MyProfileDTO
    {
        // Dados da conta
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }

        // Dados do perfil profissional
        [Required]
        [MaxLength(150)]
        public string ProfileFullName { get; set; }

        [MaxLength(20)]
        public string? ProfilePhone { get; set; }

        [MaxLength(200)]
        public string? LinkedIn { get; set; }

        [MaxLength(200)]
        public string? GitHub { get; set; }

        public string? Summary { get; set; }
        public string? Skills { get; set; }
        public string? Experience { get; set; }
        public string? Education { get; set; }

        public bool HasProfile { get; set; }
        public DateTime? ProfileCreatedAt { get; set; }
    }
}
