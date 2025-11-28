using AdmissionSystem.Models;

namespace AdmissionSystem.Services;

public interface IAuthenticationService
{
    ApplicationUser? CurrentUser { get; }
    Task<bool> LoginAsync(string email, string password);
    Task<(bool Success, string Error)> RegisterAsync(string email, string password, string firstName, string lastName, string? patronymic, DateTime? dateOfBirth, string? passportNumber, string? address);
    void Logout();
    bool IsInRole(string role);
}
