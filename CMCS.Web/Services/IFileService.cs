using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CMCS.Web.Services
{
    // Minimal interface used by controllers/services. Implement it in your project.
    public interface IFileService
    {
        // Returns (Success, ErrorMessage, StoredFilePath)
        Task<(bool Success, string Error, string StoredFilePath)> SaveFileAsync(IFormFile file, int claimId);
    }
}
