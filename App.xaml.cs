using System.Windows;
using System.Threading.Tasks;
using SmartSaving.Repositories;
using SmartSaving.Services;
using SmartSaving.ViewModels;
using SmartSaving.Views;

namespace SmartSaving
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show splash screen
            var splash = new AppSplashScreen();
            splash.Show();

            // 1. Create repositories
            var userRepository = new UserRepository();
            var accountRepository = new AccountRepository();
            var transactionRepository = new TransactionRepository();
            var categoryRepository = new CategoryRepository();

            // 2. Create services
            var authService = new AuthService(userRepository, accountRepository, categoryRepository);
            var transactionService = new TransactionService(transactionRepository, accountRepository, categoryRepository);

            // 3. Ping database in background to warm up Supabase during splash
            var warmupTask = Task.Run(async () =>
            {
                try { await userRepository.GetAllUsersAsync(); }
                catch { }
            });

            // 4. Wait for splash duration and warmup simultaneously
            await Task.WhenAll(warmupTask, Task.Delay(3000));

            // 5. Create navigation service and open login
            var navigationService = new NavigationService(authService, transactionService, categoryRepository, accountRepository, transactionRepository, userRepository);
            var loginVM = new LoginViewModel(authService, navigationService);
            var loginWindow = new LoginWindow(loginVM);
            loginWindow.Show();

            splash.Close();
        }
    }
}
