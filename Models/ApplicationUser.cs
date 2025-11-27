using Microsoft.AspNetCore.Identity;

namespace AdmissionSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public string? Address { get; set; }

    public virtual ICollection<AdmissionApplication> Applications { get; set; } = new List<AdmissionApplication>();
}
