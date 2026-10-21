using Gyro.Api.Database;

namespace Gyro.Tests.Api.Database;

public class DbTests : IDisposable
{
    public void Dispose() => Db.Close();

    [Fact]
    public void UseInitializesAnInMemoryDatabaseWithoutError()
    {
        var (journalMode, foreignKeys) = Db.Use(db => (db.Pragma("journal_mode"), db.Pragma("foreign_keys")));

        Assert.Equal("memory", journalMode);
        Assert.Equal(1L, foreignKeys);
    }

    [Fact]
    public void UseReturnsTheSameConnectionOnRepeatedCalls()
    {
        Assert.Same(Db.Use(db => db), Db.Use(db => db));
    }

    [Fact]
    public void UsesTheSqlite3MultipleCiphersEngineSoDbKeyEncryptionWorks()
    {
        var version = Db.Use(db =>
        {
            using var command = db.CreateCommand();
            command.CommandText = "SELECT sqlite3mc_version()";
            return command.ExecuteScalar();
        });

        Assert.StartsWith("SQLite3 Multiple Ciphers", version as string);
    }

    [Fact]
    public void CreateIdReturnsUniqueCuid2s()
    {
        var first = Db.CreateId();

        Assert.Equal(24, first.Length);
        Assert.NotEqual(first, Db.CreateId());
    }
}
