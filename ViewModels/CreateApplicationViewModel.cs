using System.ComponentModel.DataAnnotations;

namespace AdmissionSystem.ViewModels;

public class CreateApplicationViewModel
{
    [Required(ErrorMessage = "Выберите образовательную программу")]
    [Display(Name = "Образовательная программа")]
    public int ProgramId { get; set; }

    [Display(Name = "Информация об образовании")]
    [StringLength(500)]
    public string? EducationInfo { get; set; }

    [Display(Name = "Средний балл аттестата")]
    [Range(0, 5, ErrorMessage = "Средний балл должен быть от 0 до 5")]
    public decimal? ExamScore { get; set; }

    [Display(Name = "Дополнительная информация")]
    [StringLength(2000)]
    public string? Comments { get; set; }
}
