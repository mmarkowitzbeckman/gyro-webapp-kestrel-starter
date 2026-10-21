using Microsoft.Data.Sqlite;
using Visus.Cuid;

namespace Gyro.Api.Database;

// A single shared connection, like better-sqlite3. Access is serialized through Use() because
// Kestrel handles requests concurrently and SqliteConnection is not thread-safe.
public static class Db
{
    private static readonly Lock Gate = new();
    private static SqliteConnection? _db;

    public static T Use<T>(Func<SqliteConnection, T> work)
    {
        lock (Gate)
        {
            return work(Open());
        }
    }

    public static void Close()
    {
        lock (Gate)
        {
            _db?.Dispose();
            _db = null;
        }
    }

    public static string CreateId() => new Cuid2().ToString();

    public static void Exec(this SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public static object? Pragma(this SqliteConnection db, string pragma)
    {
        using var command = db.CreateCommand();
        command.CommandText = $"PRAGMA {pragma}";
        return command.ExecuteScalar();
    }

    private static SqliteConnection Open()
    {
        if (_db is not null)
        {
            return _db;
        }

        var dbPath = Config.String("DB_PATH", Path.Combine("data", "app.db"));
        var dbKey = Config.String("DB_KEY", "");

        var directory = Path.GetDirectoryName(dbPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Password makes Microsoft.Data.Sqlite issue a safely quoted `PRAGMA key` on open.
        var connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath, Pooling = false };

        if (dbKey.Length > 0)
        {
            connectionString.Password = dbKey;
        }

        var db = new SqliteConnection(connectionString.ToString());
        db.Open();
        db.Pragma("journal_mode = WAL");
        db.Pragma("foreign_keys = ON");

        Schema.CreateTables(db);

        _db = db;
        return _db;
    }
}
