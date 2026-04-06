using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Services;

namespace SmartSaving.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly User _currentUser;
        private Transaction? _existingTransaction;

        // ---- Form fields ----

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private string _selectedTransactionType = "Income";
        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set { _selectedTransactionType = value; OnPropertyChanged(); }
        }

        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // ---- Collections for UI binding ----

        public List<string> TransactionTypes { get; } = new List<string> { "Income", "Expense" };

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Transaction> _transactions = new();
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set { _transactions = value; OnPropertyChanged(); }
        }

        // ---- Edit mode support ----

        public bool IsEditing => _existingTransaction != null;
        public string Title => IsEditing ? "Edit Transaction" : "New Transaction";

        // ---- Commands ----

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        // Constructor for CREATING a new transaction
        public TransactionViewModel(ITransactionService transactionService, User user)
        {
            _transactionService = transactionService;
            _currentUser = user;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync);
            RefreshCommand = new AsyncRelayCommand(LoadTransactionsAsync);

            // Load the user's categories from their default account
            LoadCategories();

            // Load existing transactions
            _ = LoadTransactionsAsync();
        }

        // Constructor for EDITING an existing transaction
        public TransactionViewModel(ITransactionService transactionService, User user, Transaction transaction)
            : this(transactionService, user)
        {
            _existingTransaction = transaction;

            // Pre-fill the form with the existing transaction data
            Description = transaction.Description;
            Amount = transaction.Amount;
            Date = transaction.Date;
            CategoryId = transaction.CategoryId;
            SelectedTransactionType = transaction.Type == TransactionType.Income ? "Income" : "Expense";

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(IsEditing));
        }

        private void LoadCategories()
        {
            var defaultAccount = _currentUser.Accounts?.FirstOrDefault();
            if (defaultAccount?.Categories != null)
            {
                Categories = new ObservableCollection<Category>(defaultAccount.Categories);
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                var list = await _transactionService.GetTransactionsAsync(_currentUser.Id);
                Transactions = new ObservableCollection<Transaction>(list);
            }
            catch (Exception)
            {
                // Handle silently or log
            }
        }

        private async Task SaveAsync()
        {
            ErrorMessage = string.Empty;

            if (Amount <= 0)
            {
                ErrorMessage = "Amount must be greater than 0.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Description is required.";
                return;
            }

            try
            {
                if (IsEditing)
                {
                    _existingTransaction!.Description = Description;
                    _existingTransaction.Amount = Amount;
                    _existingTransaction.Date = Date;
                    _existingTransaction.CategoryId = CategoryId;

                    await _transactionService.UpdateTransactionAsync(_existingTransaction);
                }
                else
                {
                    var transaction = new Transaction
                    {
                        Description = Description,
                        Amount = Amount,
                        Date = Date,
                        CategoryId = CategoryId
                    };

                    await _transactionService.AddTransactionAsync(_currentUser.Id, transaction);
                }

                // Reset form after saving
                Description = string.Empty;
                Amount = 0;
                Date = DateTime.Now;
                _existingTransaction = null;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsEditing));

                // Reload transaction list
                await LoadTransactionsAsync();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred while saving.";
            }
        }

        private async Task DeleteAsync()
        {
            if (!IsEditing || _existingTransaction == null)
                return;

            try
            {
                await _transactionService.DeleteTransactionAsync(_existingTransaction.Id);

                _existingTransaction = null;
                Description = string.Empty;
                Amount = 0;
                Date = DateTime.Now;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsEditing));

                await LoadTransactionsAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while deleting.";
            }
        }
    }
}