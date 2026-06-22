 # CDL File API

  Ultra-lightweight passthrough file API. Reads and writes case documents stored
  under a single base folder on the server. No business logic — just safe file I/O.

  ## Configuration

  Set the base folder in `appsettings.json`. All file paths are relative to it;
  the base path never comes from the request.

  ```json
  "FileApi": {
    "BaseFolder": "C:\\path\\to\\documents"
  }

  Endpoints

  Base route: /files

  ┌────────┬───────────────────────────────────────┬──────────────────────────────────────────────────────┐
  │ Method │                 Route                 │                       Purpose                        │
  ├────────┼───────────────────────────────────────┼──────────────────────────────────────────────────────┤
  │ GET    │ /files/{relativePath}                 │ Download a file (streamed)                           │
  ├────────┼───────────────────────────────────────┼──────────────────────────────────────────────────────┤
  │ GET    │ /files/exists/{relativePath}          │ Get file metadata if it exists                       │
  ├────────┼───────────────────────────────────────┼──────────────────────────────────────────────────────┤
  │ POST   │ /files/{relativePath}?overwrite=false │ Upload a file (multipart/form-data, field name file) │
  └────────┴───────────────────────────────────────┴──────────────────────────────────────────────────────┘

  Read — GET /files/{relativePath}

  Streams the raw file bytes. Returns 404 if the file is missing.

  Check — GET /files/exists/{relativePath}

  Returns 200 with JSON metadata, or 404 if not found:
  { "name": "scan.pdf", "relativePath": "2026/Magiel/scan.pdf", "sizeBytes": 12345, "lastModifiedUtc":
  "2026-06-22T10:00:00Z" }

  Upload — POST /files/{relativePath}

  Saves the uploaded file under the base folder, creating any missing subfolders.
  - overwrite=false (default): if a file already exists → 409 Conflict.
  - overwrite=true: replaces the existing file.
  - Path that escapes the base folder → 400 Bad Request.
  - Success → 200 OK.

  Security

  Every read and write resolves the combined path and confirms it stays inside the
  base folder. Path traversal (../../) and absolute paths are rejected, so the API
  can never reach the rest of the server's filesystem.

  Run

  dotnet run --project CDL_File_Stealer
  Swagger UI is available at /swagger in Development.
