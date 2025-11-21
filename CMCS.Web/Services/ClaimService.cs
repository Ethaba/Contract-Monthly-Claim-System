using CMCS.Web.Data;
using CMCS.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Services
{
    public class ClaimService : IClaimService
    {
        private readonly AppDbContext _db;
        private readonly IFileService _fileService;

        public ClaimService(AppDbContext db, IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        public async Task<Claim> CreateClaimAsync(Claim claim, IEnumerable<ClaimItem> items)
        {
            if (items != null)
            {
                foreach (var it in items)
                {
                    it.Amount = it.HoursWorked * it.HourlyRate;
                    claim.ClaimItems.Add(it);
                }
            }

            claim.TotalAmount = claim.ClaimItems.Sum(i => i.Amount);

            // Run automated validation
            var validationResult = ValidateClaim(claim);
            if (!validationResult.IsValid)
            {
                claim.Status = "Rejected";
                claim.Notes = string.Join("; ", validationResult.Errors);
            }
            else
            {
                claim.Status = "Submitted";
            }

            claim.DateSubmitted = DateTime.UtcNow;

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();
            return claim;
        }

        private (bool IsValid, List<string> Errors) ValidateClaim(Claim claim)
        {
            var errors = new List<string>();

            foreach (var item in claim.ClaimItems)
            {
                if (item.HoursWorked <= 0)
                    errors.Add($"Item '{item.Description}' has invalid hours worked.");
                if (item.HourlyRate <= 0)
                    errors.Add($"Item '{item.Description}' has invalid hourly rate.");
            }

            // Example policy: max total claim R5000
            if (claim.TotalAmount > 5000)
                errors.Add($"Total claim amount {claim.TotalAmount:C} exceeds maximum allowed (R5000).");

            // Example policy: max hours per item = 12
            foreach (var item in claim.ClaimItems)
            {
                if (item.HoursWorked > 12)
                    errors.Add($"Item '{item.Description}' exceeds maximum allowed hours (12).");
            }

            return (!errors.Any(), errors);
        }

        public async Task<IEnumerable<Claim>> GetClaimsForUserAsync(int userId)
        {
            return await _db.Claims
                .Where(c => c.UserId == userId)
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.Approvals)
                    .ThenInclude(a => a.Approver)
                .ToListAsync();
        }

        public async Task<Claim?> GetClaimAsync(int id)
        {
            return await _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.Approvals)
                    .ThenInclude(a => a.Approver)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
        }

        public async Task ApproveClaimAsync(int claimId, int approverId, string role, string decision, string comments)
        {
            var claim = await _db.Claims.FindAsync(claimId);
            if (claim == null) throw new KeyNotFoundException("Claim not found");

            role = (role ?? "").Trim();
            decision = (decision ?? "").Trim();

            if (role.Equals("Coordinator", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(claim.Status, "Submitted", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Coordinator can only verify claims that are in 'Submitted' status.");

                claim.Status = decision.Equals("Approved", StringComparison.OrdinalIgnoreCase) ? "Reviewed" : "Rejected";
            }
            else if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(claim.Status, "Reviewed", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Manager can only approve/reject claims that are 'Reviewed' by a Coordinator.");

                claim.Status = decision.Equals("Approved", StringComparison.OrdinalIgnoreCase) ? "Approved" : "Rejected";
            }
            else
            {
                throw new InvalidOperationException("Approver role not supported.");
            }

            _db.Approvals.Add(new Approval
            {
                ClaimId = claimId,
                ApproverId = approverId,
                Role = role,
                Decision = claim.Status,
                DecisionAt = DateTime.UtcNow,
                Comments = comments
            });

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            return await _db.Claims
                .Include(c => c.User)
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.Approvals)
                    .ThenInclude(a => a.Approver)
                .ToListAsync();
        }
    }
}
