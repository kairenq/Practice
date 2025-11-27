using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdmissionSystem.Data;
using AdmissionSystem.Models;
using AdmissionSystem.ViewModels;

namespace AdmissionSystem.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var applications = await _context.AdmissionApplications
            .Include(a => a.Program)
            .Where(a => a.UserId == user!.Id)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return View(applications);
    }

    [HttpGet]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Programs = new SelectList(
            await _context.EducationPrograms.Where(p => p.IsActive).ToListAsync(),
            "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Create(CreateApplicationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var application = new AdmissionApplication
            {
                UserId = user!.Id,
                ProgramId = model.ProgramId,
                EducationInfo = model.EducationInfo,
                ExamScore = model.ExamScore,
                Comments = model.Comments,
                Status = ApplicationStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            _context.AdmissionApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Заявка успешно создана";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Programs = new SelectList(
            await _context.EducationPrograms.Where(p => p.IsActive).ToListAsync(),
            "Id", "Name");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var application = await _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin") && !User.IsInRole("Staff") && application.UserId != user!.Id)
        {
            return Forbid();
        }

        return View(application);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Submit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var application = await _context.AdmissionApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == user!.Id);

        if (application == null)
        {
            return NotFound();
        }

        if (application.Status != ApplicationStatus.Draft)
        {
            TempData["Error"] = "Можно подавать только черновики заявок";
            return RedirectToAction(nameof(Details), new { id });
        }

        application.Status = ApplicationStatus.Submitted;
        application.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Заявка успешно подана";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var application = await _context.AdmissionApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == user!.Id);

        if (application == null)
        {
            return NotFound();
        }

        if (application.Status != ApplicationStatus.Draft)
        {
            TempData["Error"] = "Можно удалять только черновики заявок";
            return RedirectToAction(nameof(Index));
        }

        _context.AdmissionApplications.Remove(application);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Заявка успешно удалена";
        return RedirectToAction(nameof(Index));
    }
}
