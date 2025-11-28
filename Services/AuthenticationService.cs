using Microsoft.AspNetCore.Identity;
using AdmissionSystem.Models;

namespace AdmissionSystem.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ApplicationUser? CurrentUser { get; private set; }

    public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (result.Succeeded)
        {
            CurrentUser = user;
            return true;
        }

        return false;
    }

    public async Task<(bool Success, string Error)> RegisterAsync(string email, string password, string firstName, string lastName, string? patronymic, DateTime? dateOfBirth, string? passportNumber, string? address)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Patronymic = patronymic,
            DateOfBirth = dateOfBirth?.ToUniversalTime(),
            PassportNumber = passportNumber,
            Address = address,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Applicant");
            CurrentUser = user;
            return (true, string.Empty);
        }

        return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsInRole(string role)
    {
        if (CurrentUser == null)
            return false;

        var roles = _userManager.GetRolesAsync(CurrentUser).Result;
        return roles.Contains(role);
    }
}
