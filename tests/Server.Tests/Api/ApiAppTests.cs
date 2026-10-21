using System.Net;
using System.Net.Http.Json;
using Gyro.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace Gyro.Tests.Api;

// Response bodies are asserted byte-for-byte so the API stays indistinguishable from the Express/Yoga one.
public class ApiAppTests : IAsyncLifetime
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        ApiApp.AddServices(builder.Services);

        _app = builder.Build();
        ApiApp.Configure(_app);
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();

    [Fact]
    public async Task GraphQLHealthQueryReturnsOk()
    {
        var response = await _client.PostAsJsonAsync("/graphql", new { query = "query health { health }", variables = new { } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("""{"data":{"health":"ok"}}""", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task RestHealthRouteReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("""{"status":"ok"}""", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task UnknownRouteReturnsJson404()
    {
        var response = await _client.PostAsync("/nope", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("""{"error":"Not Found"}""", await response.Content.ReadAsStringAsync());
    }
}
