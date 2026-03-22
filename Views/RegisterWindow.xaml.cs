using SmartSaving.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartSaving.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var vm = new RegisterViewModel();
            vm.OnRegisterSuccess = GoToLogin;
            this.DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = txtPassword.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.ConfirmPassword = txtConfirmPassword.Password;
        }

        public void GoToLogin()
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}