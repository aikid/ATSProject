using Domain.DTOs.Retorno;

namespace WebApp.WebAppUtilities
{
    public interface IApiClient
    {
        Task<Resultado<T>> GetAsync<T>(string caminho);
        Task<Resultado<TResp>> PostAsync<TReq, TResp>(string caminho, TReq obj);
        Task<Resultado<TResp>> PutAsync<TReq, TResp>(string caminho, TReq obj);
        Task<Resultado<TResp>> PatchAsync<TResp>(string caminho);
        Task<Resultado<TResp>> DeleteAsync<TResp>(string caminho, int id);
    }
}