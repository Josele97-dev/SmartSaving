using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using SmartSaving.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SmartSaving.Events;

namespace SmartSaving.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly User _currentUser;
        private readonly ICategoryRepository _categoryRepository;
        private Transaction? _existingTransaction;
        private readonly INavigationService _navigationService;
        private bool _isUpdating = false;

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

        private DateTime _date = DateTime.UtcNow;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private string _selectedTransactionType = "Income";
        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                _selectedTransactionType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsExpenseType));
                UpdateFilteredCategories();
            }
        }

        public Action? CloseAction { get; set; }

        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged();

                var category = Categories.FirstOrDefault(c => c.Id == value);

                if (category != null)
                {
                    // Set directly without triggering UpdateFilteredCategories again
                    _selectedTransactionType = category.Type == TransactionType.Income ? "Income" : "Expense";
                    OnPropertyChanged(nameof(SelectedTransactionType));
                }

                BudgetLimit = category?.BudgetLimit.HasValue == true
                    ? category.BudgetLimit.Value.ToString()
                    : string.Empty;

                UpdateFilteredCategories();
            }
        }
       

        private string _errorMessage = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(_errorMessage);
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        private string _budgetLimit = string.Empty;
        public string BudgetLimit
        {
            get => _budgetLimit;
            set { _budgetLimit = value; OnPropertyChanged(); }
        }

        // ---- Collections for UI binding ----

        public List<string> TransactionTypes { get; } = new List<string> { "Income", "Expense" };

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Category> _filteredCategories = new();
        public ObservableCollection<Category> FilteredCategories
        {
            get => _filteredCategories;
            set { _filteredCategories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Transaction> _transactions = new();
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set { _transactions = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoTransactions)); }
        }
        public bool HasNoTransactions => _transactions == null || !_transactions.Any();

        public bool IsExpenseType => SelectedTransactionType == "Expense";

        // ---- Edit mode support ----

        public bool IsEditing => _existingTransaction != null;
        public string Title => IsEditing ? "Edit Transaction" : "New Transaction";

        public bool IsNotEditing => !IsEditing;

        public bool IsEditableTransaction => !IsEditing ||
    (_existingTransaction?.Category?.Type == TransactionType.Expense);

        // ---- Commands ----

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        // Constructor for CREATING a new transaction
        public TransactionViewModel(ITransactionService transactionService, User user, ICategoryRepository categoryRepository, INavigationService navigationService)
        {
            _transactionService = transactionService;
            _currentUser = user;
            _categoryRepository = categoryRepository;
            _navigationService = navigationService;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync);
            RefreshCommand = new AsyncRelayCommand(LoadTransactionsAsync);

            // Load the user's categories from their default account
            _ = LoadCategoriesAsync();

            // Load existing transactions
            _ = LoadTransactionsAsync();
        }

        // Constructor for EDITING an existing transaction
        public TransactionViewModel(ITransactionService transactionService, User user, ICategoryRepository categoryRepository, INavigationService navigationService, Transaction transaction)
    : this(transactionService, user, categoryRepository, navigationService)
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

        private async Task LoadCategoriesAsync()
        {
            var defaultAccount = _currentUser.Accounts?.FirstOrDefault();
            if (defaultAccount == null) return;

            var freshCategories = await _categoryRepository.GetAllByAccountIdAsync(defaultAccount.Id);
            Categories = new ObservableCollection<Category>(freshCategories);
            UpdateFilteredCategories(); 
        }

        private void UpdateFilteredCategories()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                var type = SelectedTransactionType == "Income"
                    ? TransactionType.Income
                    : TransactionType.Expense;

                FilteredCategories = new ObservableCollection<Category>(
                    Categories.Where(c => c.Type == type));
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                var list = await _transactionService.GetTransactionsAsync(_currentUser.Id);
                Transactions = new ObservableCollection<Transaction>(list.Take(10));
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
                bool budgetExceeded = false;

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

                    // Check budget before saving
                    var category = Categories.FirstOrDefault(c => c.Id == CategoryId);
                    

                    if (category?.BudgetLimit.HasValue == true)
                    {
                        var now = DateTime.UtcNow;
                        var allTransactions = await _transactionService.GetTransactionsAsync(_currentUser.Id);
                        var spent = allTransactions
                            .Where(t => t.CategoryId == CategoryId
                                     && t.Date.Month == now.Month
                                     && t.Date.Year == now.Year)
                            .Sum(t => t.Amount);

                        if (spent + Amount > category.BudgetLimit.Value)
                        {
                            var remaining = category.BudgetLimit.Value - spent;
                            var message = $"This transaction will exceed your monthly budget for {category.Title}.\n\n" +
                                          $"Budget: €{category.BudgetLimit.Value:N2}\n" +
                                          $"Spent so far: €{spent:N2}\n" +
                                          $"Remaining: €{remaining:N2}";

                            var dialog = new BudgetWarningDialog(message);
                            dialog.ShowDialog();

                            if (dialog.Result == BudgetWarningResult.Cancel)
                                return;

                            if (dialog.Result == BudgetWarningResult.UpdateLimit)
                            {
                                _navigationService.OpenManageCategoriesWindow(_currentUser);

                                await LoadCategoriesAsync(); // changed from LoadCategories()
                                var updatedCategory = Categories.FirstOrDefault(c => c.Id == CategoryId);
                                BudgetLimit = updatedCategory?.BudgetLimit.HasValue == true
                                    ? updatedCategory.BudgetLimit.Value.ToString()
                                    : string.Empty;
                                return;

                            }
                            budgetExceeded = true;
                        }
                    }

                    await _transactionService.AddTransactionAsync(_currentUser.Id, transaction);
                }

                bool wasEditing = IsEditing;

                // Reset form after saving
                Description = string.Empty;
                Amount = 0;
                Date = DateTime.UtcNow;
                _existingTransaction = null;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsEditing));

                // Reload transaction list
                await LoadTransactionsAsync();
                EventAggregator.PublishTransactionChanged();
                CloseAction?.Invoke();

                if (budgetExceeded)
                    SuccessMessage = "⚠ Transaction saved — budget limit exceeded!";
                else
                    SuccessMessage = string.Empty;


                if (!wasEditing) {
                    if (decimal.TryParse(BudgetLimit, out decimal limit))
                    {
                        var category = Categories.FirstOrDefault(c => c.Id == CategoryId);
                        if (category != null)
                        {
                            category.BudgetLimit = limit;
                            await _categoryRepository.UpdateAsync(category);
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(BudgetLimit))
                    {
                        var category = Categories.FirstOrDefault(c => c.Id == CategoryId);
                        if (category != null)
                        {
                            category.BudgetLimit = null;
                            await _categoryRepository.UpdateAsync(category);
                        }
                    }
                }
                    
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
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
                Date = DateTime.UtcNow;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsEditing));

                await LoadTransactionsAsync();
                EventAggregator.PublishTransactionChanged();
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while deleting.";
            }
        }
    }
}