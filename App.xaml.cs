using System.Configuration;
using System.Data;
using System.Windows;
using SmartSaving.ViewModels;
using SmartSaving.Views;

namespace SmartSaving
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var authService = new AuthService();
            var navigationService = new NavigationService(authService);

            var loginVM = new LoginViewModel(authService, navigationService);
            var loginWindow = new LoginWindow(loginVM);

            loginWindow.Show();
        }
   
    }
}
