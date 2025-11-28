using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;
using AdmissionSystem.Models;

namespace AdmissionSystem.Windows;

public partial class CreateApplicationWindow : Window
{
    private readonly IAuthenticationService _authService;
    private readonly IApplicationService _applicationService;

    public CreateApplicationWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();

        LoadPrograms();
    }

    private async void LoadPrograms()
    {
        var programs = await _applicationService.GetActiveProgramsAsync();
        ProgramComboBox.ItemsSource = programs;
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Visibility = Visibility.Collapsed;

        if (ProgramComboBox.SelectedItem == null)
        {
            ShowError("Выберите программу обучения");
            return;
        }

        var program = (EducationProgram)ProgramComboBox.SelectedItem;

        decimal? examScore = null;
        if (!string.IsNullOrWhiteSpace(ExamScoreTextBox.Text))
        {
            if (decimal.TryParse(ExamScoreTextBox.Text.Replace(',', '.'), out var score))
            {
                if (score < 0 || score > 5)
                {
                    ShowError("Средний балл должен быть от 0 до 5");
                    return;
                }
                examScore = score;
            }
            else
            {
                ShowError("Неверный формат среднего балла");
                return;
            }
        }

        var application = new AdmissionApplication
        {
            UserId = _authService.CurrentUser!.Id,
            ProgramId = program.Id,
            EducationInfo = EducationInfoTextBox.Text.Trim(),
            ExamScore = examScore,
            Comments = CommentsTextBox.Text.Trim()
        };

        SubmitButton.IsEnabled = false;
        SubmitButton.Content = "Создание...";

        var success = await _applicationService.CreateApplicationAsync(application);

        if (success)
        {
            MessageBox.Show("Заявка успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        else
        {
            ShowError("Ошибка при создании заявки");
            SubmitButton.IsEnabled = true;
            SubmitButton.Content = "СОЗДАТЬ ЗАЯВКУ";
        }
    }

    private void ShowError(string message)
    {
        ErrorTextBlock.Text = message;
        ErrorTextBlock.Visibility = Visibility.Visible;
    }
}
