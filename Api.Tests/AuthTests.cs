using System.Net;
using System.Net.Http.Json;
using Domain.DTOs;
using FluentAssertions;
using Xunit;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var request = new LoginRequestDTO
        {
            USUARIO = "admin@ats.com",
            SENHA = "123456"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/autenticacao/login",
            request
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoSenhaInvalida()
    {
        var request = new LoginRequestDTO
        {
            USUARIO = "admin@ats.com",
            SENHA = "errada"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/autenticacao/login",
            request
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_DeveGerarNovoAccessToken()
    {
        // Primeiro faz login
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginRequestDTO
            {
                USUARIO = "admin@ats.com",
                SENHA = "123456"
            });

        var loginData = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponseDTO>();

        // Agora testa refresh
        var refreshResponse = await _client.PostAsJsonAsync(
            "/api/autenticacao/refresh",
            new RefreshRequestDTO
            {
                RefreshToken = loginData.RefreshToken
            });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshData = await refreshResponse.Content
            .ReadFromJsonAsync<RefreshResponseDTO>();

        refreshData.AccessToken.Should().NotBeNullOrEmpty();
    }

}
