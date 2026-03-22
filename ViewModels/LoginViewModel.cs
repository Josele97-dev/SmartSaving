using CommunityToolkit.Mvvm.Input;
using SmartSaving.Services;
using System.Windows;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public class LoginViewModel
    {
        private readonly AuthService authService = new AuthService();

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Completa todos los campos");
                return;
            }

            if (!Email.Contains("@"))
            {
                MessageBox.Show("El email no es válido");
                return;
            }

            bool success = authService.Login(Email, Password);
            if (success)
            {

                MessageBox.Show("Login correcto");
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
            }
        }
    }
}