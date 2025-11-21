using CMCS.Web.Data;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            // Defensive: exclude any users with null/empty username to avoid SQLite null-read exceptions.
            var users = await _db.Users
                .Where(u => !string.IsNullOrEmpty(u.Username))
                .OrderBy(u => u.Username)
                .ToListAsync();

            return View(users);
        }

        // POST: choose user & action
        [HttpPost]
        public async Task<IActionResult> Navigate(int userId, string actionType)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Selected user not found.";
                return RedirectToAction("Index");
            }

            // Enforce server-side role checks
            if (actionType == "Coordinator" && user.Role != "Coordinator")
            {
                TempData["Error"] = "Access denied: only Coordinators can open the Coordinator Dashboard.";
                return RedirectToAction("Index");
            }
            if (actionType == "Manager" && user.Role != "Manager")
            {
                TempData["Error"] = "Access denied: only Managers can open the Manager Dashboard.";
                return RedirectToAction("Index");
            }
            // Lecturers can only access MyClaims/CreateClaim (adjust if your rules differ)
            if ((actionType == "MyClaims" || actionType == "CreateClaim") && user.Role != "Lecturer")
            {
                TempData["Error"] = "Access denied: only Lecturers can create or view Lecturer claims.";
                return RedirectToAction("Index");
            }

            // authorized - redirect as before
            return actionType switch
            {
                "MyClaims" => RedirectToAction("Index", "Claim", new { userId }),
                "CreateClaim" => RedirectToAction("Create", "Claim", new { userId }),
                "Coordinator" => RedirectToAction("Index", "CoordinatorDashboard", new { userId }),
                "Manager" => RedirectToAction("Index", "ManagerDashboard", new { userId }),
                _ => RedirectToAction("Index")
            };
        }
    }
}
