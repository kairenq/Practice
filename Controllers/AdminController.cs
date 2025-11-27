using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdmissionSystem.Data;
using AdmissionSystem.Models;

namespace AdmissionSystem.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var applications = await _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return View(applications);
    }

    [HttpGet]
    public async Task<IActionResult> Applications(ApplicationStatus? status = null)
    {
        var query = _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var applications = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        return View(applications);
    }

    [HttpGet]
    public async Task<IActionResult> ReviewApplication(int id)
    {
        var application = await _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return NotFound();
        }

        return View(application);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status, string? comments)
    {
        var application = await _context.AdmissionApplications.FindAsync(id);

        if (application == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        application.Status = status;
        application.UpdatedAt = DateTime.UtcNow;
        application.ReviewedBy = user!.Email;
        application.ReviewedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(comments))
        {
            application.Comments = comments;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Статус заявки успешно обновлен";
        return RedirectToAction(nameof(ReviewApplication), new { id });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Programs()
    {
        var programs = await _context.EducationPrograms.ToListAsync();
        return View(programs);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Statistics()
    {
        var totalApplications = await _context.AdmissionApplications.CountAsync();
        var submittedApplications = await _context.AdmissionApplications
            .CountAsync(a => a.Status == ApplicationStatus.Submitted);
        var acceptedApplications = await _context.AdmissionApplications
            .CountAsync(a => a.Status == ApplicationStatus.Accepted);
        var rejectedApplications = await _context.AdmissionApplications
            .CountAsync(a => a.Status == ApplicationStatus.Rejected);

        var programStats = await _context.EducationPrograms
            .Select(p => new
            {
                Program = p,
                ApplicationCount = p.Applications.Count,
                AcceptedCount = p.Applications.Count(a => a.Status == ApplicationStatus.Accepted)
            })
            .ToListAsync();

        ViewBag.TotalApplications = totalApplications;
        ViewBag.SubmittedApplications = submittedApplications;
        ViewBag.AcceptedApplications = acceptedApplications;
        ViewBag.RejectedApplications = rejectedApplications;
        ViewBag.ProgramStats = programStats;

        return View();
    }
}
