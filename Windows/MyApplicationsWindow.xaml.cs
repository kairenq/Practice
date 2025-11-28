using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;
using AdmissionSystem.Models;

namespace AdmissionSystem.Windows;

public partial class MyApplicationsWindow : Window
{
    private readonly IAuthenticationService _authService;
    private readonly IApplicationService _applicationService;

    public MyApplicationsWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();

        LoadApplications();
    }

    private async void LoadApplications()
    {
        if (_authService.CurrentUser != null)
        {
            var applications = await _applicationService.GetUserApplicationsAsync(_authService.CurrentUser.Id);
            ApplicationsDataGrid.ItemsSource = applications;
        }
    }

    private void ApplicationsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ApplicationsDataGrid.SelectedItem is AdmissionApplication application)
        {
            MessageBox.Show(
                $"Заявка №{application.Id}\n" +
                $"Программа: {application.Program.Name}\n" +
                $"Статус: {application.Status}\n" +
                $"Средний балл: {application.ExamScore?.ToString("F2") ?? "-"}\n" +
                $"Информация об образовании: {application.EducationInfo ?? "-"}\n" +
                $"Комментарии: {application.Comments ?? "-"}",
                "Детали заявки",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
