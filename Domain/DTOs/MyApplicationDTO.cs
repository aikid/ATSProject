namespace Domain.DTOs
{
    public class MyApplicationDTO
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string JobDescription { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public bool JobIsActive { get; set; }
    }
}
