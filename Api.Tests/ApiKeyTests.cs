using System.Net;
using FluentAssertions;
using Xunit;


namespace Api.Tests
{
    public class ApiKeyTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiKeyTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task EndpointProtegido_DeveRetornar401_SemApiKey()
        {
            var response = await _client.GetAsync("/api/teste/naoprotegido");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EndpointProtegido_DeveRetornar200_ComApiKeyValida()
        {
            _client.DefaultRequestHeaders.Add("X-API-KEY", "ATS-LOCAL-DEV-KEY-123456");

            var response = await _client.GetAsync("/api/teste/protegido");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
