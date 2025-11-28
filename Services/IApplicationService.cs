using AdmissionSystem.Models;

namespace AdmissionSystem.Services;

public interface IApplicationService
{
    Task<List<EducationProgram>> GetActiveProgramsAsync();
    Task<List<AdmissionApplication>> GetUserApplicationsAsync(string userId);
    Task<List<AdmissionApplication>> GetAllApplicationsAsync();
    Task<AdmissionApplication?> GetApplicationByIdAsync(int id);
    Task<bool> CreateApplicationAsync(AdmissionApplication application);
    Task<bool> UpdateApplicationStatusAsync(int id, ApplicationStatus status, string reviewedBy, string? comments);
    Task<bool> SubmitApplicationAsync(int id);
    Task<bool> DeleteApplicationAsync(int id);
    Task<Dictionary<string, int>> GetStatisticsAsync();
}
