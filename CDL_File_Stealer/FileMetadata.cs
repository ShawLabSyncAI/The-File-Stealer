namespace CDL_File_Stealer
{
    //just the return dto for the exist endpoint.
    public class FileMetadata
    {
        public string Name { get; set; } = string.Empty;

        public string RelativePath { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public DateTime LastModifiedUtc { get; set; }
    }
}
