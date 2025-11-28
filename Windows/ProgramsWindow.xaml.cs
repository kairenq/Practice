using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;

namespace AdmissionSystem.Windows;

public partial class ProgramsWindow : Window
{
    private readonly IApplicationService _applicationService;

    public ProgramsWindow()
    {
        InitializeComponent();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();

        LoadPrograms();
    }

    private async void LoadPrograms()
    {
        var programs = await _applicationService.GetActiveProgramsAsync();
        ProgramsDataGrid.ItemsSource = programs;
    }
}
