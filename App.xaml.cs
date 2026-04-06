using System.Windows;
using SmartSaving.Repositories;
using SmartSaving.Services;
using SmartSaving.ViewModels;
using SmartSaving.Views;

namespace SmartSaving
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Create repositories
            var userRepository = new UserRepository();
            var accountRepository = new AccountRepository();
            var transactionRepository = new TransactionRepository();

            // 2. Create services (inject repositories)
            var authService = new AuthService(userRepository, accountRepository);
            var transactionService = new TransactionService(transactionRepository, accountRepository);

            // 3. Create navigation service (inject services)
            var navigationService = new NavigationService(authService, transactionService);

            // 4. Open the login window
            var loginVM = new LoginViewModel(authService, navigationService);
            var loginWindow = new LoginWindow(loginVM);
            loginWindow.Show();
        }
    }
}
