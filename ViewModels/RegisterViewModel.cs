using CommunityToolkit.Mvvm.Input;
using SmartSaving.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public class RegisterViewModel
    {
        private readonly AuthService authService = new AuthService();

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Action OnRegisterSuccess { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Completa todos los campos");
                return;
            }

            if (!Email.Contains("@"))
            {
                MessageBox.Show("El email no es válido");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                return;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("La contraseña debe tener al menos 6 caracteres");
                return;
            }

            bool success = authService.Register(Email, Password, FirstName, LastName);
            if (success)
            {
                MessageBox.Show("Usuario registrado correctamente");
                OnRegisterSuccess?.Invoke();
            }
            else
            {
                MessageBox.Show("El usuario ya existe");
            }
        }
    }
}