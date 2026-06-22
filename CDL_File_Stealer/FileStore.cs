namespace CDL_File_Stealer
{
    // Reads and writes case documents under a single base folder. Registered once as a singleton since no state.
    public class FileStore
    {
        private readonly string _baseRoot;
        //this gets injected at Program.cs which comes from the appsettings, "BaseFolder"
        public FileStore(string basePath)
        {
            _baseRoot = Path.GetFullPath(basePath);
        }

        // Combines base + relative path and returns it only if the result stays inside the base folder.
        public string? ResolveSafePath(string relativePath)
        {
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(Path.Combine(_baseRoot, relativePath));
            }
            catch
            {
                return null;
            }

            string baseWithSeparator = _baseRoot + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(baseWithSeparator, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return fullPath;
        }

        //Checjs if it is exists? 
        public bool Exists(string relativePath)
        {
            string? fullPath = ResolveSafePath(relativePath);
            if (fullPath == null)
            {
                return false;
            }

            return File.Exists(fullPath);
        }

        //this primarily for Check endpoint.
        public FileMetadata GetMetadata(string relativePath)
        {
            string fullPath = ResolveSafePath(relativePath)!;
            FileInfo info = new FileInfo(fullPath);

            return new FileMetadata
            {
                Name = info.Name,
                RelativePath = relativePath,
                SizeBytes = info.Length,
                LastModifiedUtc = info.LastWriteTimeUtc
            };
        }

        //this is for handling the file to byets.
        public FileStream? OpenRead(string relativePath)
        {
            string? fullPath = ResolveSafePath(relativePath);
            if (fullPath == null || !File.Exists(fullPath))
            {
                return null;
            }

            return File.OpenRead(fullPath);
        }

        //well, this is for POST.
        public async Task SaveAsync(string relativePath, IFormFile file)
        {
            string fullPath = ResolveSafePath(relativePath)!;
            string directory = Path.GetDirectoryName(fullPath)!;
            Directory.CreateDirectory(directory);

            await using FileStream destination = File.Create(fullPath);
            await file.CopyToAsync(destination);
        }
    }
}
