namespace Gyro.Api.Routes;

public static class IndexRoutes
{
    public static void MapIndexRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/health", () => Results.Json(new { status = "ok" }));
    }
}
