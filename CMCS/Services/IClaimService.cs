using CMCS.Models;

namespace CMCS.Services
{
    public interface IClaimService
    {
        Claim CreateClaim(Claim claim, IEnumerable<ClaimItem> items);
        IEnumerable<Claim> GetClaimsForUser(int userId);
        Claim GetClaimWithDetails(int claimId);
        IEnumerable<Claim> GetPendingClaims();
        void ApproveClaim(int claimId, int approverUserId, string role, string decision, string comments = null);
    }
}
