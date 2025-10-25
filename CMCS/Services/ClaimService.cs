using CMCS.Data;
using CMCS.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Services
{
    public class ClaimService : IClaimService
    {
        private readonly AppDbContext _db;
        public ClaimService(AppDbContext db)
        {
            _db = db;
        }

        public Claim CreateClaim(Claim claim, IEnumerable<ClaimItem> items)
        {
            decimal total = 0;
            foreach (var it in items)
            {
                it.Amount = it.HoursWorked * it.HourlyRate;
                total += it.Amount;
            }
            claim.TotalAmount = total;
            claim.DateSubmitted = DateTime.UtcNow;
            claim.Status = "Submitted";

            _db.Claims.Add(claim);
            _db.SaveChanges();

            foreach (var it in items)
            {
                it.ClaimId = claim.ClaimId;
                _db.ClaimItems.Add(it);
            }
            _db.SaveChanges();

            return claim;
        }

        public IEnumerable<Claim> GetClaimsForUser(int userId)
        {
            return _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.Approvals)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();
        }

        public Claim GetClaimWithDetails(int claimId)
        {
            return _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.Approvals)
                .Include(c => c.User)
                .FirstOrDefault(c => c.ClaimId == claimId);
        }

        public IEnumerable<Claim> GetPendingClaims()
        {
            return _db.Claims
                .Include(c => c.User)
                .Where(c => c.Status == "Submitted" || c.Status == "Reviewed")
                .OrderBy(c => c.DateSubmitted).ToList();
        }

        public void ApproveClaim(int claimId, int approverUserId, string role, string decision, string comments = null)
        {
            var approval = new Approval
            {
                ClaimId = claimId,
                ApproverUserId = approverUserId,
                Role = role,
                Decision = decision,
                DecisionAt = DateTime.UtcNow,
                Comments = comments
            };
            _db.Approvals.Add(approval);

            var claim = _db.Claims.Find(claimId);
            if (claim == null) throw new Exception("Claim not found");

            if (decision == "Approved")
            {
                if (role == "Manager") claim.Status = "Approved";
                else if (role == "Coordinator") claim.Status = "Reviewed";
            }
            else if (decision == "Rejected")
            {
                claim.Status = "Rejected";
            }

            _db.SaveChanges();
        }
    }
}
