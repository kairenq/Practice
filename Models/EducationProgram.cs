using System.ComponentModel.DataAnnotations;

namespace AdmissionSystem.Models;

public class EducationProgram
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string Code { get; set; } = string.Empty;

    public int DurationYears { get; set; }

    [Required]
    [StringLength(100)]
    public string EducationLevel { get; set; } = string.Empty; // СПО, Бакалавриат и т.д.

    public int PlacesCount { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<AdmissionApplication> Applications { get; set; } = new List<AdmissionApplication>();
}
