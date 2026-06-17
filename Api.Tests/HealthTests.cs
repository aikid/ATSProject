using System.Net;
using FluentAssertions;
using Xunit;

namespace Api.Tests
{
    public class HealthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public HealthTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Health_DeveRetornarHealthy()
        {
            var response = await _client.GetAsync("/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Healthy");
        }

    }
}
