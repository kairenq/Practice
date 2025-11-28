using System.Windows;
using System.Windows.Documents;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;

namespace AdmissionSystem.Windows;

public partial class LoginWindow : Window
{
    private readonly IAuthenticationService _authService;

    public LoginWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Visibility = Visibility.Collapsed;

        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Заполните все поля");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Content = "Вход...";

        var success = await _authService.LoginAsync(email, password);

        if (success)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        else
        {
            ShowError("Неверный email или пароль");
            LoginButton.IsEnabled = true;
            LoginButton.Content = "ВОЙТИ";
        }
    }

    private void RegisterLink_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.Show();
        this.Close();
    }

    private void ShowError(string message)
    {
        ErrorTextBlock.Text = message;
        ErrorTextBlock.Visibility = Visibility.Visible;
    }
}
