using Domain.Model;

namespace Domain.DTOs.Retorno
{
    public sealed class Resultado<T>
    {
        public bool Sucesso { get; }
        public T Dado { get; }
        public RetornoErro Erro { get; }

        private Resultado(T dado)
        {
            Sucesso = true;
            Dado = dado;
        }

        private Resultado(RetornoErro erro)
        {
            Sucesso = false;
            Erro = erro;
        }

        public static Resultado<T> Ok(T dado) => new(dado);
        public static Resultado<T> Fail(RetornoErro erro) => new(erro);
    }
}
