using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Services;

namespace SmartSaving.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ITransactionService _transactionService;
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

        public string WelcomeMessage => $"Welcome, {_currentUser.FirstName}!";

        public ICommand OpenTransactionCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel(INavigationService navigationService, ITransactionService transactionService, User user)
        {
            _navigationService = navigationService;
            _transactionService = transactionService;
            _currentUser = user;

            OpenTransactionCommand = new RelayCommand(OpenTransaction);
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);

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

                RecentTransactions = new ObservableCollection<Transaction>(transactions);

                TotalIncome = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                TotalExpenses = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                Balance = TotalIncome - TotalExpenses;
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
    }
}