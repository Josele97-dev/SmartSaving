using SmartSaving.Commands;
using SmartSaving.Events;
using SmartSaving.Models;
using SmartSaving.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public  class MonthlyTransactionsViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly INavigationService _navigationService;
        private readonly User _currentUser;

        public string MonthTitle => $"{DateTime.Now:MMMM yyyy}";

        private ObservableCollection<Transaction> _incomeTransactions = new();
        public ObservableCollection<Transaction> IncomeTransactions
        {
            get => _incomeTransactions;
            set { _incomeTransactions = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoIncome)); }
        }
        private Transaction? _selectedTransaction;
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }

        public ICommand OpenTransactionCommand { get; }

        private ObservableCollection<Transaction> _expenseTransactions = new();
        public ObservableCollection<Transaction> ExpenseTransactions
        {
            get => _expenseTransactions;
            set { _expenseTransactions = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoExpenses)); }
        }


        public bool HasNoIncome => _incomeTransactions == null || !_incomeTransactions.Any();
        public bool HasNoExpenses => _expenseTransactions == null || !_expenseTransactions.Any();

        public MonthlyTransactionsViewModel(ITransactionService transactionService, INavigationService navigationService, User user)
        {
            _transactionService = transactionService;
            _navigationService = navigationService;
            _currentUser = user;


            OpenTransactionCommand = new RelayCommand(OpenTransaction);
            EventAggregator.TransactionChanged += async () => await LoadDataAsync();
            _ = LoadDataAsync();
        }

        private void OpenTransaction()
        {
            if (SelectedTransaction == null) return;
            _navigationService.OpenTransactionWindow(_currentUser, SelectedTransaction);
        }
        private async Task LoadDataAsync()
        {
            try
            {
                var now = DateTime.Now;
                var all = await _transactionService.GetTransactionsAsync(_currentUser.Id);

                var thisMonth = all
                    .Where(t => t.Date.Month == now.Month && t.Date.Year == now.Year)
                    .ToList();

                IncomeTransactions = new ObservableCollection<Transaction>(
                    thisMonth.Where(t => t.Type == TransactionType.Income)
                             .OrderByDescending(t => t.Date));

                ExpenseTransactions = new ObservableCollection<Transaction>(
                    thisMonth.Where(t => t.Type == TransactionType.Expense)
                             .OrderByDescending(t => t.Date));
            }
            catch (Exception)
            {
                
            }
        }


    }
}
