using System.ComponentModel.DataAnnotations;

namespace AdmissionSystem.Models;

public class AdmissionApplication
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    public int ProgramId { get; set; }
    public virtual EducationProgram Program { get; set; } = null!;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(500)]
    public string? EducationInfo { get; set; } // Предыдущее образование

    public decimal? ExamScore { get; set; }

    [StringLength(2000)]
    public string? Comments { get; set; }

    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
