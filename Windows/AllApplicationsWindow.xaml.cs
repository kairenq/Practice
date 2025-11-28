using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;
using AdmissionSystem.Models;

namespace AdmissionSystem.Windows;

public partial class AllApplicationsWindow : Window
{
    private readonly IAuthenticationService _authService;
    private readonly IApplicationService _applicationService;

    public AllApplicationsWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();

        LoadApplications();
    }

    private async void LoadApplications()
    {
        var applications = await _applicationService.GetAllApplicationsAsync();
        ApplicationsDataGrid.ItemsSource = applications;
    }

    private async void ApplicationsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ApplicationsDataGrid.SelectedItem is AdmissionApplication application)
        {
            var reviewWindow = new ReviewApplicationWindow(application.Id);
            reviewWindow.ShowDialog();
            LoadApplications(); // Refresh after review
        }
    }
}
