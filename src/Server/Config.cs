namespace Gyro;

public static class Config
{
    // Empty values fall back too, matching `process.env.X || fallback`.
    public static string String(string name, string fallback) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value ? value : fallback;

    public static int Port(string name, int fallback) =>
        int.TryParse(Environment.GetEnvironmentVariable(name), out var port) ? port : fallback;
}
