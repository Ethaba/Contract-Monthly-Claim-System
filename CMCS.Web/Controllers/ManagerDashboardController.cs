using CMCS.Web.Data;
using CMCS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers
{
    public class ManagerDashboardController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly AppDbContext _db;

        public ManagerDashboardController(IClaimService claimService, AppDbContext db)
        {
            _claimService = claimService;
            _db = db;
        }

        // GET: /ManagerDashboard?userId=3
        public async Task<IActionResult> Index(int userId = 0)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            if (user.Role != "Manager")
            {
                TempData["Error"] = "Access denied: only Managers can access this dashboard.";
                return RedirectToAction("Index", "Home");
            }

            // Managers see claims that have been verified or still submitted (example filter)
            var allClaims = await _claimService.GetAllClaimsAsync();
            var toReview = allClaims.Where(c => c.Status == "Submitted" || c.Status == "Reviewed").ToList();
            ViewBag.UserId = userId;
            return View(toReview);
        }
    }
}
