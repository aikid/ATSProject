namespace WebApp.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
    }
}
