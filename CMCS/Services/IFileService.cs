namespace CMCS.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Save a local file (sourcePath) into managed storage for the claim.
        /// Returns (Success, ErrorMessage?, StoredFilePath?).
        /// </summary>
        (bool Success, string? Error, string? StoredFilePath) SaveFile(string sourcePath, int claimId);

        bool IsAllowedExtension(string fileName);
        long MaxFileSizeBytes { get; }
    }
}
