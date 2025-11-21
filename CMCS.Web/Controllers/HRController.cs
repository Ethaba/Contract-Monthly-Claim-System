using CMCS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers
{
    public class HRController : Controller
    {
        private readonly IClaimService _claimService;

        public HRController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        // GET: /HR/ApprovedClaims
        public async Task<IActionResult> ApprovedClaims()
        {
            var allClaims = await _claimService.GetAllClaimsAsync();
            var approvedClaims = allClaims
                .Where(c => string.Equals(c.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.User?.Username)
                .ThenBy(c => c.ClaimYear)
                .ThenBy(c => c.ClaimMonth)
                .ToList();


            return View(approvedClaims);
        }
    }
}
