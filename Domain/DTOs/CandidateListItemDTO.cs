namespace Domain.DTOs
{
    public class CandidateListItemDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public bool HasProfile { get; set; }
        public int ApplicationCount { get; set; }
    }
}
