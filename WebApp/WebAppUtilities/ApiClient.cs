using Domain.DTOs.Retorno;
using Domain.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebApp.WebAppUtilities
{
    public sealed class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _config;

        private readonly string _apiToken;
        private readonly string _apiPath;

        public ApiClient(HttpClient http, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _http = http;
            _httpContext = httpContext;
            _config = config;

            _apiToken = _config.GetSection("WebAppUtil:apiEspecialistaToken").Value;
            _apiPath = _config.GetSection("WebAppUtil:apiATSPath").Value;
        }

        private void AplicarHeaders()
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _http.DefaultRequestHeaders.Add("token", _apiToken);
            _http.DefaultRequestHeaders.Add("tokenuser", AutenticacaoUtil.ObterHashUser(_httpContext));
        }

        public async Task<Resultado<T>> GetAsync<T>(string caminho)
        {
            AplicarHeaders();

            string caminhoCompleto = $"{_apiPath}{caminho}";

            using var retorno = await _http.GetAsync(caminhoCompleto);
            var conteudo = await retorno.Content.ReadAsStringAsync();

            if (retorno.IsSuccessStatusCode)
            {
                var dado = JsonConvert.DeserializeObject<T>(conteudo);
                return Resultado<T>.Ok(dado);
            }

            var erro = TryParseErro(conteudo) ?? new RetornoErro { Mensagem = $"Erro {(int)retorno.StatusCode} - {retorno.ReasonPhrase}", Erro = conteudo };
            return Resultado<T>.Fail(erro);
        }

        public async Task<Resultado<TResp>> PostAsync<TReq, TResp>(string caminho, TReq obj)
        {
            AplicarHeaders();

            string caminhoCompleto = $"{_apiPath}{caminho}";

            var json = JsonConvert.SerializeObject(obj);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var retorno = await _http.PostAsync(caminhoCompleto, content);
            var conteudo = await retorno.Content.ReadAsStringAsync();

            if (retorno.IsSuccessStatusCode)
            {
                var dado = JsonConvert.DeserializeObject<TResp>(conteudo);
                return Resultado<TResp>.Ok(dado);
            }

            var erro = TryParseErro(conteudo) ?? new RetornoErro { Mensagem = $"Erro {(int)retorno.StatusCode} - {retorno.ReasonPhrase}", Erro = conteudo };
            return Resultado<TResp>.Fail(erro);
        }

        public async Task<Resultado<TResp>> DeleteAsync<TResp>(string caminho, int id)
        {
            AplicarHeaders();

            string caminhoCompleto = $"{_apiPath}{caminho}/{id}";

            using var requ = new HttpRequestMessage(HttpMethod.Delete, caminhoCompleto);
            using var retorno = await _http.SendAsync(requ);
            var conteudo = await retorno.Content.ReadAsStringAsync();

            if (retorno.IsSuccessStatusCode)
                return Resultado<TResp>.Ok(default);

            var erro = TryParseErro(conteudo) ?? new RetornoErro
            {
                Mensagem = $"Erro {(int)retorno.StatusCode} - {retorno.ReasonPhrase}",
                Erro = conteudo
            };

            return Resultado<TResp>.Fail(erro);
        }

        private static RetornoErro TryParseErro(string body)
        {
            try
            {
                return JsonConvert.DeserializeObject<RetornoErro>(body);
            }
            catch
            {
                return null;
            }
        }
    }
}