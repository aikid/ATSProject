namespace Domain.DTOs
{
    public class CadastroRequestDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Senha { get; set; }
        public string SenhaConfirmacao { get; set; }
    }
}
