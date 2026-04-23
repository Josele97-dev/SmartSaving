using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using SmartSaving.Events;

namespace SmartSaving.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ITransactionService _transactionService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private Timer? _pollingTimer;
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

        private decimal _monthlyIncome;
        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set { _monthlyIncome = value; OnPropertyChanged(); }
        }

        private decimal _monthlyExpenses;
        public decimal MonthlyExpenses
        {
            get => _monthlyExpenses;
            set { _monthlyExpenses = value; OnPropertyChanged(); }
        }

        private decimal _monthlyNet;
        public decimal MonthlyNet
        {
            get => _monthlyNet;
            set { _monthlyNet = value; OnPropertyChanged(); OnPropertyChanged(nameof(MonthlyNetColour)); }
        }

        public string MonthlyNetColour => _monthlyNet >= 0 ? "#FF27AE60" : "#FFE74C3C";

        private DateTime _lastRefreshed;
        public DateTime LastRefreshed
        {
            get => _lastRefreshed;
            set { _lastRefreshed = value; OnPropertyChanged(); OnPropertyChanged(nameof(LastRefreshedText)); }
        }

        public string LastRefreshedText => $"Last updated: {_lastRefreshed:dd/MM/yyyy HH:mm}";

        public ObservableCollection<Transaction> RecentTransactions
        {
            get => _recentTransactions;
            set { _recentTransactions = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoTransactions)); }
        }
        public bool HasNoTransactions => RecentTransactions == null || !RecentTransactions.Any();

        private Transaction? _selectedTransaction;
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }
        private int _monthlyTransactionCount;
        public int MonthlyTransactionCount
        {
            get => _monthlyTransactionCount;
            set { _monthlyTransactionCount = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CategoryBudgetProgress> _categoryProgress = new();
        public ObservableCollection<CategoryBudgetProgress> CategoryProgress
        {
            get => _categoryProgress;
            set { _categoryProgress = value; OnPropertyChanged(); }
        }
        private string _transferBanner = string.Empty;
        public string TransferBanner
        {
            get => _transferBanner;
            set { _transferBanner = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowTransferBanner)); }
        }

        public bool ShowTransferBanner => !string.IsNullOrEmpty(_transferBanner);
        public string WelcomeMessage
        {
            get
            {
                var hour = DateTime.Now.Hour;
                var greeting = hour < 12 ? "Good morning"
                             : hour < 18 ? "Good afternoon"
                             : "Good evening";
                return $"{greeting}, {_currentUser.FirstName}!";
            }

        }

        public ICommand OpenTransactionCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand LogoutCommand { get; }

        public ICommand EditTransactionCommand { get; }

        public ICommand ManageCategoriesCommand { get; }

        public ICommand SearchCommand { get; }
        public ICommand ViewMonthlyCommand { get; }
        public ICommand TransferCommand { get; }
        public ICommand TransfersRecordCommand { get; }

        public MainViewModel(INavigationService navigationService, ITransactionService transactionService, IAccountRepository accountRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository, User user)
        {
            _navigationService = navigationService;
            _transactionService = transactionService;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository; // add this
            _transactionRepository = transactionRepository;
            _currentUser = user;

            OpenTransactionCommand = new RelayCommand(OpenTransaction);
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            LogoutCommand = new RelayCommand(Logout);
            EditTransactionCommand = new RelayCommand(EditTransaction);
            ManageCategoriesCommand = new RelayCommand(OpenManageCategories);
            SearchCommand = new RelayCommand(OpenSearch);
            ViewMonthlyCommand = new RelayCommand(OpenMonthlyTransactions);
            TransferCommand = new RelayCommand(OpenTransfer);
            TransfersRecordCommand = new RelayCommand(OpenTransfersRecord);

            // Load balance from the user's default account
            var defaultAccount = user.Accounts?.FirstOrDefault();
            Balance = defaultAccount?.CurrentBalance ?? 0;

            EventAggregator.TransactionChanged += async () => await LoadDataAsync();
            EventAggregator.CategoryChanged += async () => await LoadDataAsync();
            _pollingTimer = new Timer(async _ => await CheckForIncomingTransfersAsync(),
             null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));


            // Fire-and-forget initial data load (safe here because errors are caught internally)
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Re-fetch account from database for fresh balance
                var freshAccount = await _accountRepository.GetDefaultByUserIdAsync(_currentUser.Id);
                if (freshAccount != null)
                    Balance = freshAccount.CurrentBalance;

                var transactions = await _transactionService.GetTransactionsAsync(_currentUser.Id);
                RecentTransactions = new ObservableCollection<Transaction>(transactions.Take(10));

                TotalIncome = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                TotalExpenses = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                Balance = TotalIncome - TotalExpenses;

                var now = DateTime.Now;
                var thisMonth = transactions
                    .Where(t => t.Date.Month == now.Month && t.Date.Year == now.Year)
                    .ToList();

                MonthlyIncome = thisMonth
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                MonthlyExpenses = thisMonth
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                MonthlyNet = MonthlyIncome - MonthlyExpenses;
                MonthlyTransactionCount = thisMonth.Count();

                // Load category budget progress
                if (freshAccount != null)
                {
                    var categories = await _categoryRepository.GetByAccountIdAsync(freshAccount.Id);
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

                // Update last refreshed time
                LastRefreshed = DateTime.Now;
            }
            catch (System.Exception)
            {
                // Silently handle
            }
        }
        private async Task CheckForIncomingTransfersAsync()
        {
            try
            {
                var defaultAccount = await _accountRepository.GetDefaultByUserIdAsync(_currentUser.Id);
                if (defaultAccount == null) return;

                var unread = await _transactionRepository.GetUnreadTransferInsAsync(defaultAccount.Id);
                if (unread.Count == 0) return;

                await _transactionRepository.MarkTransfersAsReadAsync(defaultAccount.Id);

                var latest = unread.First();
                var description = latest.Description ?? string.Empty;
                var senderName = description.Contains(" \u2190 ")
                    ? description.Substring(description.IndexOf(" \u2190 ") + 3).Trim()
                    : "someone";

                App.Current.Dispatcher.Invoke(() =>
                {
                    TransferBanner = $"💸 €{latest.Amount:N2} received from {senderName}!";
                });

                await Task.Delay(3000);

                App.Current.Dispatcher.Invoke(() =>
                {
                    TransferBanner = string.Empty;
                });

                await LoadDataAsync();
            }
            catch (Exception)
            {
                // Handle silently
            }
        }
        private void OpenTransaction()
        {
            _navigationService.OpenTransactionWindow(_currentUser);
        }
        private void Logout()  // add here
        {
            _pollingTimer?.Dispose();

            EventAggregator.TransactionChanged -= async () => await LoadDataAsync();
            EventAggregator.CategoryChanged -= async () => await LoadDataAsync();

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

        private void OpenMonthlyTransactions()
        {
            _navigationService.OpenMonthlyTransactionsWindow(_currentUser);
        }


        private void OpenTransfer()
        {
            _navigationService.OpenTransferWindow(_currentUser);
        }

    
        private void OpenTransfersRecord()
        {
            _navigationService.OpenTransfersRecordWindow(_currentUser);
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