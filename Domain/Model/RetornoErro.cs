namespace Domain.Model
{
    public class RetornoErro
    {
        public string Titulo { get; set; } = "Erro";
        public string Mensagem { get; set; } = "Não foi possível concluir a operação.";
        public string Erro { get; set; }
    }
}
