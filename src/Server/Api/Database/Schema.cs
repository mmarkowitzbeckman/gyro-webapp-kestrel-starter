using Microsoft.Data.Sqlite;

namespace Gyro.Api.Database;

public static class Schema
{
    public static void CreateTables(SqliteConnection db)
    {
        db.Exec(
            """
            -- Define your tables here, e.g.:
            --
            -- CREATE TABLE IF NOT EXISTS widgets (
            --   id TEXT PRIMARY KEY,
            --   name TEXT NOT NULL,
            --   created_at TEXT NOT NULL DEFAULT (datetime('now')),
            --   updated_at TEXT NOT NULL DEFAULT (datetime('now'))
            -- );
            """
        );

        // Still on the initial schema version — new columns go straight into the
        // CREATE TABLE statements above. Once real deployed data exists and tables
        // can't just be recreated, migrate existing databases like this instead:
        //
        // using var command = db.CreateCommand();
        // command.CommandText = "SELECT name FROM pragma_table_info('widgets')";
        // using var reader = command.ExecuteReader();
        // var widgetColumns = new List<string>();
        // while (reader.Read()) widgetColumns.Add(reader.GetString(0));
        // if (!widgetColumns.Contains("some_new_column"))
        // {
        //     db.Exec("ALTER TABLE widgets ADD COLUMN some_new_column TEXT");
        // }
    }
}
