using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AdmissionSystem.Models;

namespace AdmissionSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<EducationProgram> EducationPrograms { get; set; }
    public DbSet<AdmissionApplication> AdmissionApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AdmissionApplication>()
            .HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AdmissionApplication>()
            .HasOne(a => a.Program)
            .WithMany(p => p.Applications)
            .HasForeignKey(a => a.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EducationProgram>().HasData(
            new EducationProgram
            {
                Id = 1,
                Name = "Программирование в компьютерных системах",
                Code = "09.02.03",
                Description = "Подготовка специалистов по разработке программного обеспечения",
                DurationYears = 3,
                EducationLevel = "СПО",
                PlacesCount = 25,
                IsActive = true
            },
            new EducationProgram
            {
                Id = 2,
                Name = "Информационные системы и программирование",
                Code = "09.02.07",
                Description = "Подготовка специалистов в области ИТ",
                DurationYears = 3,
                EducationLevel = "СПО",
                PlacesCount = 30,
                IsActive = true
            },
            new EducationProgram
            {
                Id = 3,
                Name = "Сетевое и системное администрирование",
                Code = "09.02.06",
                Description = "Подготовка системных администраторов",
                DurationYears = 3,
                EducationLevel = "СПО",
                PlacesCount = 20,
                IsActive = true
            }
        );
    }
}
