using Domain.DTOs;
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

        private readonly string _apiPath;

        public ApiClient(HttpClient http, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _http = http;
            _httpContext = httpContext;
            _config = config;

            _apiPath = _config.GetSection("WebAppUtil:apiATSPath").Value;
        }

        private void AplicarHeaders()
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var accessToken = _httpContext.HttpContext?.Request.Cookies["ACCESS_TOKEN"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }


        public async Task<Resultado<T>> GetAsync<T>(string caminho)
        {
            AplicarHeaders();

            string caminhoCompleto = $"{_apiPath}{caminho}";

            using var retorno = await _http.GetAsync(caminhoCompleto);

            if (retorno.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await TentarRefreshAsync())
                {
                    AplicarHeaders();
                    retorno.Dispose();

                    using var retry = await _http.GetAsync(caminhoCompleto);
                    return await ProcessarResposta<T>(retry);
                }
            }

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

            if (retorno.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await TentarRefreshAsync())
                {
                    AplicarHeaders();
                    retorno.Dispose();

                    using var retry = await _http.PostAsync(caminhoCompleto, content);
                    return await ProcessarResposta<TResp>(retry);
                }
            }

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

        private async Task<bool> TentarRefreshAsync()
        {
            var refreshToken = _httpContext.HttpContext?.Request.Cookies["REFRESH_TOKEN"];

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var json = JsonConvert.SerializeObject(new
            {
                RefreshToken = refreshToken
            });

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync($"{_apiPath}api/autenticacao/refresh", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var body = await response.Content.ReadAsStringAsync();
            var tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(body);

            var responseCookies = _httpContext.HttpContext.Response;

            var isDev = _httpContext.HttpContext.Request.IsHttps == false;

            responseCookies.Cookies.Append("ACCESS_TOKEN", tokens.AccessToken, CookieUtil.AccessToken(isDev));

            responseCookies.Cookies.Append("REFRESH_TOKEN", tokens.RefreshToken, CookieUtil.RefreshToken(isDev));

            return true;
        }

        private async Task<Resultado<T>> ProcessarResposta<T>(HttpResponseMessage response)
        {
            var conteudo = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var dado = JsonConvert.DeserializeObject<T>(conteudo);
                return Resultado<T>.Ok(dado);
            }

            var erro = TryParseErro(conteudo)
                ?? new RetornoErro { Mensagem = "Erro na API", Erro = conteudo };

            return Resultado<T>.Fail(erro);
        }


    }
}