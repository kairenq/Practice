using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;

namespace AdmissionSystem.Windows;

public partial class StatisticsWindow : Window
{
    private readonly IApplicationService _applicationService;

    public StatisticsWindow()
    {
        InitializeComponent();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();

        LoadStatistics();
    }

    private async void LoadStatistics()
    {
        var stats = await _applicationService.GetStatisticsAsync();

        TotalTextBlock.Text = stats["Total"].ToString();
        SubmittedTextBlock.Text = stats["Submitted"].ToString();
        UnderReviewTextBlock.Text = stats["UnderReview"].ToString();
        AcceptedTextBlock.Text = stats["Accepted"].ToString();
        RejectedTextBlock.Text = stats["Rejected"].ToString();
    }
}
