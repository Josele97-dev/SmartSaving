using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.ViewModels;
using SmartSaving.Views;


namespace SmartSaving.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthService _authService;
        private readonly ITransactionService _transactionService;
        private readonly ICategoryRepository _categoryRepository;
        public NavigationService(IAuthService authService, ITransactionService transactionService, ICategoryRepository categoryRepository)
        {
            _authService = authService;
            _transactionService = transactionService;
            _categoryRepository = categoryRepository;
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

        public void OpenTransactionWindow(User user, Transaction? transaction = null)
        {
            var vm = transaction != null
       ? new TransactionViewModel(_transactionService, user, _categoryRepository, transaction)
        : new TransactionViewModel(_transactionService, user, _categoryRepository);
            var window = new TransactionWindow(vm);
            window.Show();
        }
    }
}
