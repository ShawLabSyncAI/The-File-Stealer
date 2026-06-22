using Microsoft.AspNetCore.Mvc;

namespace CDL_File_Stealer.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly FileStore _fileStore;
        //This is also passed from program.cs, which comes from Appsettings. we dont turn into a string this time.
        public FilesController(FileStore basePath)
        {
            _fileStore = basePath;
        }

        //this one returns the file.
        [HttpGet("{*relativePath}")]
        public IActionResult Read(string relativePath)
        {
            FileStream? stream = _fileStore.OpenRead(relativePath);
            if (stream == null)
            {
                return NotFound();
            }

            return File(stream, "application/octet-stream", Path.GetFileName(relativePath));
        }

        //Returns some metadata about the path.
        [HttpGet("exists/{*relativePath}")]
        public IActionResult Check(string relativePath)
        {
            if (!_fileStore.Exists(relativePath))
            {
                return NotFound();
            }

            return Ok(_fileStore.GetMetadata(relativePath));
        }

        //this one is for uploading, has 1 param. if the overwrite arg is true, then it replaces the file.
        [HttpPost("{*relativePath}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(string relativePath, IFormFile file, bool overwrite = false)
        {
            if (_fileStore.ResolveSafePath(relativePath) == null)
            {
                return BadRequest("Invalid path.");
            }

            if (_fileStore.Exists(relativePath) && !overwrite)
            {
                return Conflict("THERE IS A FILE WITH THE SAME NAME!");
            }
            if (_fileStore.Exists(relativePath) && overwrite)
            {
                await _fileStore.SaveAsync(relativePath, file);
                return Ok("YOUR FILE HAS BEEN REPLACED, THERE IS NO TAKEBACK!");
            }


            await _fileStore.SaveAsync(relativePath, file);
            return Ok();
        }
    }
}
