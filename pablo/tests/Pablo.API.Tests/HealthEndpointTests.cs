using System.Net;

namespace Pablo.API.Tests;

public class HealthEndpointTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
{
    [Fact]
    public async Task Get_health_returns_healthy()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", body);
    }
}