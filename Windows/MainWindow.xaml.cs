using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;

namespace AdmissionSystem.Windows;

public partial class MainWindow : Window
{
    private readonly IAuthenticationService _authService;

    public MainWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();

        LoadUserInfo();
        BuildMenu();
    }

    private void LoadUserInfo()
    {
        var user = _authService.CurrentUser;
        if (user != null)
        {
            UserNameTextBlock.Text = $"{user.FirstName} {user.LastName}";
        }
    }

    private void BuildMenu()
    {
        MenuPanel.Children.Clear();

        if (_authService.IsInRole("Applicant"))
        {
            AddMenuButton("Мои заявки", () => new MyApplicationsWindow().Show());
            AddMenuButton("Подать заявку", () => new CreateApplicationWindow().Show());
            AddMenuButton("Программы обучения", () => new ProgramsWindow().Show());
        }

        if (_authService.IsInRole("Staff") || _authService.IsInRole("Admin"))
        {
            AddMenuButton("Все заявки", () => new AllApplicationsWindow().Show());
            AddMenuButton("Программы", () => new ProgramsWindow().Show());
        }

        if (_authService.IsInRole("Admin"))
        {
            AddMenuButton("Статистика", () => new StatisticsWindow().Show());
        }
    }

    private void AddMenuButton(string text, Action action)
    {
        var button = new Button
        {
            Content = text,
            Margin = new Thickness(0, 0, 0, 8),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 40
        };
        button.Click += (s, e) => action();
        MenuPanel.Children.Add(button);
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        _authService.Logout();
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }
}
