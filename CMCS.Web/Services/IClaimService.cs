using CMCS.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMCS.Web.Services
{
    public interface IClaimService
    {
        Task<Claim> CreateClaimAsync(Claim claim, IEnumerable<ClaimItem> items);
        Task<IEnumerable<Claim>> GetClaimsForUserAsync(int userId);
        Task<Claim?> GetClaimAsync(int id);
        Task ApproveClaimAsync(int claimId, int approverId, string role, string decision, string comments);

        // helper to retrieve all claims for HR/overview pages
        Task<IEnumerable<Claim>> GetAllClaimsAsync();
    }
}
