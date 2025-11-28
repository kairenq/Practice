using Microsoft.EntityFrameworkCore;
using AdmissionSystem.Data;
using AdmissionSystem.Models;

namespace AdmissionSystem.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EducationProgram>> GetActiveProgramsAsync()
    {
        return await _context.EducationPrograms
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<List<AdmissionApplication>> GetUserApplicationsAsync(string userId)
    {
        return await _context.AdmissionApplications
            .Include(a => a.Program)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AdmissionApplication>> GetAllApplicationsAsync()
    {
        return await _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<AdmissionApplication?> GetApplicationByIdAsync(int id)
    {
        return await _context.AdmissionApplications
            .Include(a => a.Program)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> CreateApplicationAsync(AdmissionApplication application)
    {
        try
        {
            application.CreatedAt = DateTime.UtcNow;
            application.Status = ApplicationStatus.Draft;
            _context.AdmissionApplications.Add(application);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateApplicationStatusAsync(int id, ApplicationStatus status, string reviewedBy, string? comments)
    {
        try
        {
            var application = await _context.AdmissionApplications.FindAsync(id);
            if (application == null)
                return false;

            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;
            application.ReviewedBy = reviewedBy;
            application.ReviewedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(comments))
                application.Comments = comments;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SubmitApplicationAsync(int id)
    {
        try
        {
            var application = await _context.AdmissionApplications.FindAsync(id);
            if (application == null || application.Status != ApplicationStatus.Draft)
                return false;

            application.Status = ApplicationStatus.Submitted;
            application.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteApplicationAsync(int id)
    {
        try
        {
            var application = await _context.AdmissionApplications.FindAsync(id);
            if (application == null || application.Status != ApplicationStatus.Draft)
                return false;

            _context.AdmissionApplications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var stats = new Dictionary<string, int>
        {
            ["Total"] = await _context.AdmissionApplications.CountAsync(),
            ["Submitted"] = await _context.AdmissionApplications.CountAsync(a => a.Status == ApplicationStatus.Submitted),
            ["UnderReview"] = await _context.AdmissionApplications.CountAsync(a => a.Status == ApplicationStatus.UnderReview),
            ["Accepted"] = await _context.AdmissionApplications.CountAsync(a => a.Status == ApplicationStatus.Accepted),
            ["Rejected"] = await _context.AdmissionApplications.CountAsync(a => a.Status == ApplicationStatus.Rejected)
        };

        return stats;
    }
}
