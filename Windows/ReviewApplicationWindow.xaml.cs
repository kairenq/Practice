using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;
using AdmissionSystem.Models;

namespace AdmissionSystem.Windows;

public partial class ReviewApplicationWindow : Window
{
    private readonly IAuthenticationService _authService;
    private readonly IApplicationService _applicationService;
    private readonly int _applicationId;
    private AdmissionApplication? _application;

    public ReviewApplicationWindow(int applicationId)
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
        _applicationService = App.ServiceProvider.GetRequiredService<IApplicationService>();
        _applicationId = applicationId;

        LoadApplication();
        LoadStatuses();
    }

    private async void LoadApplication()
    {
        _application = await _applicationService.GetApplicationByIdAsync(_applicationId);
        if (_application == null)
        {
            MessageBox.Show("Заявка не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
            return;
        }

        TitleTextBlock.Text = $"Заявка №{_application.Id}";

        UserInfoTextBlock.Text =
            $"ФИО: {_application.User.LastName} {_application.User.FirstName} {_application.User.Patronymic}\n" +
            $"Email: {_application.User.Email}\n" +
            $"Телефон: {_application.User.PhoneNumber ?? "-"}\n" +
            $"Дата рождения: {_application.User.DateOfBirth?.ToString("dd.MM.yyyy") ?? "-"}\n" +
            $"Паспорт: {_application.User.PassportNumber ?? "-"}\n" +
            $"Адрес: {_application.User.Address ?? "-"}";

        ProgramInfoTextBlock.Text =
            $"Название: {_application.Program.Name}\n" +
            $"Код: {_application.Program.Code}\n" +
            $"Уровень: {_application.Program.EducationLevel}\n" +
            $"Мест: {_application.Program.PlacesCount}";

        ApplicationInfoTextBlock.Text =
            $"Статус: {_application.Status}\n" +
            $"Дата создания: {_application.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}\n" +
            $"Предыдущее образование: {_application.EducationInfo ?? "-"}\n" +
            $"Средний балл: {_application.ExamScore?.ToString("F2") ?? "-"}\n" +
            $"Комментарии: {_application.Comments ?? "-"}\n" +
            (string.IsNullOrEmpty(_application.ReviewedBy) ? "" : $"Проверено: {_application.ReviewedBy} ({_application.ReviewedAt?.ToLocalTime():dd.MM.yyyy HH:mm})");
    }

    private void LoadStatuses()
    {
        StatusComboBox.ItemsSource = new[]
        {
            ApplicationStatus.Submitted,
            ApplicationStatus.UnderReview,
            ApplicationStatus.Accepted,
            ApplicationStatus.Rejected
        };
        StatusComboBox.SelectedIndex = 0;
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_application == null || StatusComboBox.SelectedItem == null)
            return;

        var newStatus = (ApplicationStatus)StatusComboBox.SelectedItem;
        var comments = ReviewCommentsTextBox.Text.Trim();

        UpdateButton.IsEnabled = false;
        UpdateButton.Content = "Обновление...";

        var success = await _applicationService.UpdateApplicationStatusAsync(
            _applicationId,
            newStatus,
            _authService.CurrentUser!.Email!,
            comments
        );

        if (success)
        {
            MessageBox.Show("Статус успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            MessageBox.Show("Ошибка при обновлении статуса", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateButton.IsEnabled = true;
            UpdateButton.Content = "ОБНОВИТЬ СТАТУС";
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
