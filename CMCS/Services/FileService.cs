using System;
using System.IO;
using System.Linq;

namespace CMCS.Services
{
    public class FileService : IFileService
    {
        private readonly string _baseDir;
        private readonly string[] _allowed = new[] { ".pdf", ".docx", ".xlsx" };
        private readonly long _maxBytes = 10 * 1024 * 1024; // 10 MB

        public FileService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _baseDir = Path.Combine(appData, "CMCS", "uploads");
            if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
        }

        public long MaxFileSizeBytes => _maxBytes;

        public bool IsAllowedExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
            return _allowed.Contains(ext);
        }

        public (bool Success, string? Error, string? StoredFilePath) SaveFile(string sourcePath, int claimId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                    return (false, "Source file not found", null);

                var fi = new FileInfo(sourcePath);
                if (fi.Length > _maxBytes) return (false, "File too large", null);
                if (!IsAllowedExtension(fi.Name)) return (false, "File type not allowed", null);

                var claimDir = Path.Combine(_baseDir, claimId.ToString());
                if (!Directory.Exists(claimDir)) Directory.CreateDirectory(claimDir);

                var unique = $"{Guid.NewGuid()}{fi.Extension}";
                var dest = Path.Combine(claimDir, unique);
                File.Copy(sourcePath, dest);

                return (true, null, dest);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
