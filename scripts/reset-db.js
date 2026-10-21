import "dotenv/config";
import fs from "node:fs";
import path from "node:path";

// Mirrors src/Server/Api/Database/Db.cs's own path resolution, so this always
// targets whichever database the server would actually open.
const DB_PATH = process.env.DB_PATH || path.join("data", "app.db");

for (const file of [DB_PATH, `${DB_PATH}-shm`, `${DB_PATH}-wal`]) {
  if (fs.existsSync(file)) {
    fs.rmSync(file);
    console.log(`Removed ${file}`);
  }
}

console.log("Database reset — tables are recreated on next server start.");
