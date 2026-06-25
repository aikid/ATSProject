namespace Domain.DTOs
{
    public class CandidateDetailDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }

        // Perfil
        public string? ProfileFullName { get; set; }
        public string? LinkedIn { get; set; }
        public string? GitHub { get; set; }
        public string? Summary { get; set; }
        public string? Skills { get; set; }
        public string? Experience { get; set; }
        public string? Education { get; set; }
        public DateTime? ProfileCreatedAt { get; set; }

        // Candidaturas
        public List<CandidateApplicationDTO> Applications { get; set; } = new();
    }

    public class CandidateApplicationDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
