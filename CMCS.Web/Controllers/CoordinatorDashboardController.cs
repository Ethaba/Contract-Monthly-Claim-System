using CMCS.Web.Data;
using CMCS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers
{
    public class CoordinatorDashboardController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly AppDbContext _db;

        public CoordinatorDashboardController(IClaimService claimService, AppDbContext db)
        {
            _claimService = claimService;
            _db = db;
        }

        // GET: /CoordinatorDashboard?userId=2
        public async Task<IActionResult> Index(int userId = 0)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            if (user.Role != "Coordinator")
            {
                TempData["Error"] = "Access denied: only Coordinators can access this dashboard.";
                return RedirectToAction("Index", "Home");
            }

            // Show claims that are submitted and waiting verification
            var allClaims = await _claimService.GetAllClaimsAsync();
            var pending = allClaims.Where(c => c.Status == "Submitted").ToList();
            ViewBag.UserId = userId;
            return View(pending);
        }
    }
}
