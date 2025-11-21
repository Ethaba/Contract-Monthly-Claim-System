using Microsoft.AspNetCore.Hosting;

namespace CMCS.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        public long MaxFileSizeBytes => 10 * 1024 * 1024;
        public FileService(IWebHostEnvironment env) { _env = env; }

        public bool IsAllowedExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? "";
            return ext == ".pdf" || ext == ".docx" || ext == ".xlsx";
        }

        public async Task<(bool Success, string Error, string StoredFilePath)> SaveFileAsync(IFormFile file, int claimId)
        {
            if (file == null) return (false, "No file", "");
            if (!IsAllowedExtension(file.FileName)) return (false, "Not allowed", "");
            if (file.Length > MaxFileSizeBytes) return (false, "Too large", "");

            var uploads = Path.Combine(_env.WebRootPath, "uploads", claimId.ToString());
            Directory.CreateDirectory(uploads);
            var dest = Path.Combine(uploads, Path.GetFileName(file.FileName));
            using var fs = new FileStream(dest, FileMode.Create);
            await file.CopyToAsync(fs);
            var webPath = $"/uploads/{claimId}/{Path.GetFileName(file.FileName)}";
            return (true, "", webPath);
        }
    }
}
