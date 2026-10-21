using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace Gyro.Tests;

public class HttpsRedirectTests
{
    [Fact]
    public async Task RedirectsPlainHttpToTheHttpsPortKeepingPathAndQuery()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        await using var app = builder.Build();
        app.UseHttpsRedirect(3003);
        await app.StartAsync();

        var response = await app.GetTestClient().GetAsync("/some/page?x=1");

        Assert.Equal(HttpStatusCode.PermanentRedirect, response.StatusCode);
        Assert.Equal("https://localhost:3003/some/page?x=1", response.Headers.Location?.ToString());
    }
}
