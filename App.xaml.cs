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

            // 1.Se muestra la Splash Screen
            var splash = new AppSplashScreen();
            splash.Show();

            // 2. Se crean los repositories
            var userRepository = new UserRepository();
            var accountRepository = new AccountRepository();
            var transactionRepository = new TransactionRepository();
            var categoryRepository = new CategoryRepository();

            // 3. Se crean los services
            var authService = new AuthService(userRepository, accountRepository, categoryRepository);
            var transactionService = new TransactionService(transactionRepository, accountRepository, categoryRepository);

            // 4. Se pingea a Supabase para que vaya arrancando la DB mientras ya ha empezado la splash
            var warmupTask = Task.Run(async () =>
            {
                try { await userRepository.GetAllUsersAsync(); }
                catch { }
            });

            // 5. Se espera el timepo determinado para la splash 
            await Task.WhenAll(warmupTask, Task.Delay(3000));

            // 6. CS crea el servicio de navegacion a la vez que login ejecuta
            var navigationService = new NavigationService(authService, transactionService, categoryRepository, accountRepository, transactionRepository, userRepository);
            var loginVM = new LoginViewModel(authService, navigationService);
            var loginWindow = new LoginWindow(loginVM);
            loginWindow.Show();

            splash.Close();
        }
    }
}
