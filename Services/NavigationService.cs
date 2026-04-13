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
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        public NavigationService(IAuthService authService, ITransactionService transactionService, ICategoryRepository categoryRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
            
        {
            _authService = authService;
            _transactionService = transactionService;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
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
            var vm = new MainViewModel(this, _transactionService, _accountRepository, _categoryRepository, user);
            var window = new MainWindow(vm);
            window.Show();
        }

        public void OpenTransactionWindow(User user, Transaction? transaction = null)
        {
            var vm = transaction != null
       ? new TransactionViewModel(_transactionService, user, _categoryRepository, this, transaction)
        : new TransactionViewModel(_transactionService, user, _categoryRepository, this);
            var window = new TransactionWindow(vm);
            window.Show();
        }
        public void OpenManageCategoriesWindow(User user)
        {
            var vm = new ManageCategoriesViewModel(_categoryRepository, user);
            var window = new ManageCategoriesWindow(vm);
            window.ShowDialog();
        }
        public void OpenSearchWindow(User user)
        {
            var vm = new SearchViewModel(_transactionRepository,this, user);
            var window = new SearchWindow(vm);
            window.Show();
        }
    }
}
