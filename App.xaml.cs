using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using AdmissionSystem.Data;
using AdmissionSystem.Models;
using AdmissionSystem.Services;

namespace AdmissionSystem;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static IConfiguration Configuration { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Build configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // Setup DI
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        // Ensure database is created and seeded
        await InitializeDatabaseAsync();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Services
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IApplicationService, ApplicationService>();
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            await context.Database.MigrateAsync();
            await SeedData.Initialize(scope.ServiceProvider, userManager, roleManager);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
