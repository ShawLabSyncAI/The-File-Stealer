# CDL File API

Ultra-lightweight passthrough file API. Reads and writes case documents stored under a single base folder on the server. No business logic — just safe, streamed file I/O behind a token check.

## Configuration

All settings live in `appsettings.json`. File paths are always relative to `BaseFolder`; the base path never comes from the request.

```json
"Kestrel": {
  "Endpoints": {
    "Http": { "Url": "http://0.0.0.0:31031" }
  }
},
"FileApi": {
  "BaseFolder": "C:\\path\\to\\documents",
  "ApiToken": "a-long-random-token"
}
```

- **`BaseFolder`** — the one folder everything is read from / written under. Use a dedicated folder, not a broad path.
- **`ApiToken`** — the permanent token every request must send.
- **`Kestrel` endpoint** — `0.0.0.0:31031` listens on all interfaces, so it's reachable on the LAN without knowing the VM's IP. (This governs the VM; `launchSettings.json` keeps `localhost` for local dev.)

## Authentication

Every request must send the token in a header:

```
X-Api-Token: <your token>
```

A missing or wrong token returns `401`. In Swagger, use the **Authorize** button to set it once.

## Endpoints

Base route: `/files`

| Method | Route | Purpose |
|--------|-------|---------|
| `GET`  | `/files/{relativePath}` | Download a file (streamed) |
| `GET`  | `/files/exists/{relativePath}` | Get file metadata if it exists |
| `POST` | `/files/{relativePath}?overwrite=false` | Upload a file (`multipart/form-data`, field name `file`) |

### Read — `GET /files/{relativePath}`
Streams the raw file bytes. Returns `404` if the file is missing.

### Check — `GET /files/exists/{relativePath}`
Returns `200` with JSON metadata, or `404` if not found:
```json
{ "name": "scan.pdf", "relativePath": "2026/Magiel/scan.pdf", "sizeBytes": 12345, "lastModifiedUtc": "2026-06-22T10:00:00Z" }
```

### Upload — `POST /files/{relativePath}`
Saves the uploaded file under the base folder, creating any missing subfolders.
- `overwrite=false` (default) and the file exists → `409 Conflict`.
- `overwrite=true` and the file exists → replaces it, `200 OK`.
- New file → `200 OK`.
- Path that escapes the base folder → `400 Bad Request`.

## Security

Every read and write resolves the combined path and confirms it stays inside the base folder. Path traversal (`../../`) and absolute paths are rejected, so the API can never reach the rest of the server's filesystem.

## Run

```
dotnet run --project CDL_File_Stealer
```

Swagger UI is available at `/swagger` in Development. On the VM the app listens on `http://0.0.0.0:31031`.
