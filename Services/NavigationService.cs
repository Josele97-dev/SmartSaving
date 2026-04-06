using SmartSaving.Views;
using SmartSaving.ViewModels;
using SmartSaving.Models;


namespace SmartSaving.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthService _authService;
        private readonly ITransactionService _transactionService;

        public NavigationService(IAuthService authService, ITransactionService transactionService)
        {
            _authService = authService;
            _transactionService = transactionService;
        }

        public void OpenLoginWindow()
        {
            var vm = new LoginViewModel(_authService, this);
            var window = new LoginWindow(vm);
            window.Show();
        }

        public void OpenRegisterWindow()
        {
            var vm = new RegisterViewModel(_authService, this);
            var window = new RegisterWindow(vm);
            window.Show();
        }

        public void OpenMainWindow(User user)
        {
            var vm = new MainViewModel(this, _transactionService, user);
            var window = new MainWindow(vm);
            window.Show();
        }

        public void OpenTransactionWindow(User user)
        {
            var vm = new TransactionViewModel(_transactionService, user);
            var window = new TransactionWindow(vm);
            window.Show();
        }
    }
}
