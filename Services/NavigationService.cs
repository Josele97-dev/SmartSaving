using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.ViewModels;
using SmartSaving.Views;
using System.Security.Cryptography.X509Certificates;
using System.Windows;


namespace SmartSaving.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthService _authService;
        private readonly ITransactionService _transactionService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        public NavigationService(IAuthService authService, ITransactionService transactionService, ICategoryRepository categoryRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUserRepository userRepository)
            
        {
            _authService = authService;
            _transactionService = transactionService;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
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

            foreach (Window w in Application.Current.Windows)
            {
                if (w is LoginWindow)
                {
                    w.Close();
                    break;
                }
            }
        }

        public void OpenMainWindow(User user)
        {
            var vm = new MainViewModel(this, _transactionService, _accountRepository, _categoryRepository, _transactionRepository, user);
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

        public void OpenMonthlyTransactionsWindow(User user)
        {
            var vm = new MonthlyTransactionsViewModel(_transactionService,this,  user);
            var window = new MonthlyTransactionsWindow(vm);
            window.Show();
        }

        public void OpenTransferWindow(User user)
        {
            var vm = new TransferViewModel(_transactionService, _userRepository, user);
            var window = new TransferWindow(vm);
            window.Show();
        }
        public void OpenTransfersRecordWindow(User user)
        {
            var vm = new TransfersRecordViewModel(_transactionService, user);
            var window = new TransfersRecordWindow(vm);
            window.Show();
        }
        public  async Task  ShowTransferNotificationIfNeeded(User user)
        {
            var defaultAccount = user.Accounts?.FirstOrDefault();
            if (defaultAccount == null) return;

            var unread = await _transactionRepository.GetUnreadTransferInsAsync(defaultAccount.Id);
            if (unread.Count == 0) return;

            var vm = new TransferNotificationViewModel(_transactionRepository, defaultAccount.Id, unread);
            var window = new TransferNotificationWindow(vm);
            window.Topmost = true;
            window.Show();
            window.Activate();
        }
    }
}
