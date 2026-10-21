using System.Runtime.CompilerServices;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Gyro.Tests;

internal static class TestSetup
{
    [ModuleInitializer]
    internal static void UseInMemoryDatabase() => Environment.SetEnvironmentVariable("DB_PATH", ":memory:");
}
