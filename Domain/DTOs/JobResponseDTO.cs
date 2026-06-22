namespace Domain.DTOs
{
    public class JobResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; }
    }
}
