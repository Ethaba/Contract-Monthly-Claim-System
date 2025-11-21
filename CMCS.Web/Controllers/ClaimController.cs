using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;
        private readonly AppDbContext _db;

        public ClaimController(IClaimService claimService, IFileService fileService, AppDbContext db)
        {
            _claimService = claimService;
            _fileService = fileService;
            _db = db;
        }

        public async Task<IActionResult> Index(int userId = 1)
        {
            var claims = await _claimService.GetClaimsForUserAsync(userId);
            ViewBag.UserId = userId;
            return View(claims);
        }

        public IActionResult Create(int userId = 1)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, Claim model, List<IFormFile>? files,
            [FromForm] List<string>? itemDescriptions, [FromForm] List<decimal>? itemHours, [FromForm] List<decimal>? itemRates)
        {
            model.UserId = userId;

            var items = new List<ClaimItem>();
            if (itemDescriptions != null)
            {
                decimal total = 0;
                for (int i = 0; i < itemDescriptions.Count; i++)
                {
                    var desc = itemDescriptions[i];
                    var hours = (itemHours != null && itemHours.Count > i) ? itemHours[i] : 0m;
                    var rate = (itemRates != null && itemRates.Count > i) ? itemRates[i] : 0m;

                    items.Add(new ClaimItem { Description = desc, HoursWorked = hours, HourlyRate = rate, Amount = hours * rate });
                    total += hours * rate;
                }
                model.TotalAmount = total;
            }

            var created = await _claimService.CreateClaimAsync(model, items);

            if (created.Status == "Rejected" && !string.IsNullOrWhiteSpace(created.Notes))
            {
                TempData["Error"] = $"Claim rejected automatically due to validation errors: {created.Notes}";
            }
            else
            {
                TempData["Success"] = "Claim submitted successfully.";
            }

            // Save uploaded files as before
            if (files != null && files.Any())
            {
                foreach (var f in files)
                {
                    var r = await _fileService.SaveFileAsync(f, created.ClaimId);
                    if (r.Success)
                    {
                        _db.SupportingDocuments.Add(new SupportingDocument
                        {
                            ClaimId = created.ClaimId,
                            FileName = Path.GetFileName(f.FileName),
                            FilePath = r.StoredFilePath,
                            FileType = Path.GetExtension(f.FileName)
                        });
                    }
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { userId });
        }


        public async Task<IActionResult> Details(int id, int? userId = null)
        {
            var claim = await _claimService.GetClaimAsync(id);
            if (claim == null) return NotFound();

            int uid = userId ?? 0;
            ViewBag.UserId = uid;

            if (uid != 0)
            {
                var user = await _db.Users.FindAsync(uid);
                ViewBag.UserRole = user?.Role;
            }
            else
            {
                ViewBag.UserRole = null;
            }

            return View(claim);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, int approverId, string role, string decision, string comments)
        {
            try
            {
                await _claimService.ApproveClaimAsync(id, approverId, role, decision, comments);
                TempData["Success"] = "Action completed successfully.";
            }
            catch
            {
                TempData["Error"] = "Could not complete action.";
            }

            return RedirectToAction("Details", new { id });
        }
    }
}
