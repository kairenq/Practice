using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AdmissionSystem.Services;

namespace AdmissionSystem.Windows;

public partial class RegisterWindow : Window
{
    private readonly IAuthenticationService _authService;

    public RegisterWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthenticationService>();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Visibility = Visibility.Collapsed;

        var firstName = FirstNameTextBox.Text.Trim();
        var lastName = LastNameTextBox.Text.Trim();
        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password;
        var confirmPassword = ConfirmPasswordBox.Password;

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Заполните все обязательные поля (*)");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Пароли не совпадают");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Пароль должен быть не менее 6 символов");
            return;
        }

        RegisterButton.IsEnabled = false;
        RegisterButton.Content = "Регистрация...";

        var result = await _authService.RegisterAsync(
            email,
            password,
            firstName,
            lastName,
            PatronymicTextBox.Text.Trim(),
            DateOfBirthPicker.SelectedDate,
            PassportTextBox.Text.Trim(),
            AddressTextBox.Text.Trim()
        );

        if (result.Success)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        else
        {
            ShowError($"Ошибка регистрации: {result.Error}");
            RegisterButton.IsEnabled = true;
            RegisterButton.Content = "ЗАРЕГИСТРИРОВАТЬСЯ";
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }

    private void ShowError(string message)
    {
        ErrorTextBlock.Text = message;
        ErrorTextBlock.Visibility = Visibility.Visible;
    }
}
