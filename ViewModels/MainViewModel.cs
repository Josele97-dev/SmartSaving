using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ITransactionService _transactionService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly User _currentUser;

        private decimal _balance;
        private decimal _totalIncome;
        private decimal _totalExpenses;
        private ObservableCollection<Transaction> _recentTransactions = new();

        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(); }
        }

        public decimal TotalIncome
        {
            get => _totalIncome;
            set { _totalIncome = value; OnPropertyChanged(); }
        }

        public decimal TotalExpenses
        {
            get => _totalExpenses;
            set { _totalExpenses = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Transaction> RecentTransactions
        {
            get => _recentTransactions;
            set { _recentTransactions = value; OnPropertyChanged(); }
        }

        private Transaction? _selectedTransaction;
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CategoryBudgetProgress> _categoryProgress = new();
        public ObservableCollection<CategoryBudgetProgress> CategoryProgress
        {
            get => _categoryProgress;
            set { _categoryProgress = value; OnPropertyChanged(); }
        }


        public string WelcomeMessage => $"Welcome, {_currentUser.FirstName}!";

        public ICommand OpenTransactionCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand LogoutCommand { get; }

        public ICommand EditTransactionCommand { get; }

        public ICommand ManageCategoriesCommand { get; }

        public ICommand SearchCommand { get; }

        public MainViewModel(INavigationService navigationService, ITransactionService transactionService, IAccountRepository accountRepository, ICategoryRepository categoryRepository, User user)
        {
            _navigationService = navigationService;
            _transactionService = transactionService;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository; // add this
            _currentUser = user;

            OpenTransactionCommand = new RelayCommand(OpenTransaction);
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            LogoutCommand = new RelayCommand(Logout);
            EditTransactionCommand = new RelayCommand(EditTransaction);
            ManageCategoriesCommand = new RelayCommand(OpenManageCategories);
            SearchCommand = new RelayCommand(OpenSearch);

            // Load balance from the user's default account
            var defaultAccount = user.Accounts?.FirstOrDefault();
            Balance = defaultAccount?.CurrentBalance ?? 0;

            // Fire-and-forget initial data load (safe here because errors are caught internally)
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsAsync(_currentUser.Id);

                RecentTransactions = RecentTransactions = new ObservableCollection<Transaction>(transactions.Take(10)); ;

                TotalIncome = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                TotalExpenses = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                Balance = TotalIncome - TotalExpenses;

                var defaultAccount = await _accountRepository.GetDefaultByUserIdAsync(_currentUser.Id);
                if (defaultAccount != null)
                {
                    var categories = await _categoryRepository.GetByAccountIdAsync(defaultAccount.Id);
                    var progressList = new ObservableCollection<CategoryBudgetProgress>();

                    foreach (var category in categories)
                    {
                        var spent = transactions
                            .Where(t => t.CategoryId == category.Id
                                     && t.Date.Month == DateTime.Now.Month
                                     && t.Date.Year == DateTime.Now.Year)
                            .Sum(t => t.Amount);

                        progressList.Add(new CategoryBudgetProgress
                        {
                            CategoryName = category.Title,
                            Spent = spent,
                            Limit = category.BudgetLimit!.Value
                        });
                    }

                    CategoryProgress = progressList;
                }
            }
            catch (System.Exception)
            {
                // Silently handle — in a real app you'd log this or show an error
            }
        }

        private void OpenTransaction()
        {
            _navigationService.OpenTransactionWindow(_currentUser);
        }
        private void Logout()  // add here
        {
            _navigationService.OpenLoginWindow();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
        private void EditTransaction()
        {
            if (SelectedTransaction == null) return;
            _navigationService.OpenTransactionWindow(_currentUser, SelectedTransaction);
        }

        private void OpenManageCategories()
        {
            _navigationService.OpenManageCategoriesWindow(_currentUser);
        }

        private void OpenSearch()
        {
            _navigationService.OpenSearchWindow(_currentUser);
        }
    }
    public class CategoryBudgetProgress
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Spent { get; set; }
        public decimal Limit { get; set; }
        public string Summary => $"{CategoryName}: €{Spent:N2} / €{Limit:N2}";
        public double Percentage => Limit > 0 ? (double)(Spent / Limit) * 100 : 0;
        public string ProgressColour => Percentage >= 100 ? "#FFE74C3C"
                                      : Percentage >= 80 ? "#FFF39C12"
                                      : "#FF27AE60";
    }
}