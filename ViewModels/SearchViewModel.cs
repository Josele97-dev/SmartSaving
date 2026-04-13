using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly User _currentUser;
        private readonly INavigationService _navigationService;

        // ---- Search fields ----

        private string _keyword = string.Empty;
        public string Keyword
        {
            get => _keyword;
            set { _keyword = value; OnPropertyChanged(); }
        }

        private int? _selectedCategoryId;
        public int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set { _selectedCategoryId = value; OnPropertyChanged(); }
        }

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged(); }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set { _fromDate = value; OnPropertyChanged(); }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set { _toDate = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // ---- Collections ----

        public List<string> TransactionTypes { get; } = new List<string> { "All", "Income", "Expense" };

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Transaction> _results = new();
        public ObservableCollection<Transaction> Results
        {
            get => _results;
            set { _results = value; OnPropertyChanged(); }
        }

        private Transaction? _selectedTransaction;
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }


        // ---- Commands ----

        public ICommand SearchCommand { get; }

        public ICommand OpenTransactionCommand { get; }

        public SearchViewModel(ITransactionRepository transactionRepository, INavigationService navigationService, User user)
        {
            _transactionRepository = transactionRepository;
            _navigationService = navigationService;
            _currentUser = user;

            SearchCommand = new AsyncRelayCommand(SearchAsync);
            OpenTransactionCommand = new RelayCommand(OpenTransaction);

            // Load categories for the dropdown
            LoadCategories();
        }

        private void LoadCategories()
        {
            var defaultAccount = _currentUser.Accounts?.FirstOrDefault();
            if (defaultAccount?.Categories != null)
            {
                // Add an "All Categories" option at the top
                var allCategories = new ObservableCollection<Category>();
                allCategories.Add(new Category { Id = 0, Title = "All Categories" });
                foreach (var cat in defaultAccount.Categories)
                    allCategories.Add(cat);

                Categories = allCategories;
                SelectedCategoryId = 0;
            }
        }

        private async Task SearchAsync()
        {
            ErrorMessage = string.Empty;

            try
            {
                int? categoryId = SelectedCategoryId == 0 ? null : SelectedCategoryId;

                TransactionType? type = SelectedType switch
                {
                    "Income" => TransactionType.Income,
                    "Expense" => TransactionType.Expense,
                    _ => null
                };

                var results = await _transactionRepository.SearchAsync(
                    _currentUser.Id,
                    string.IsNullOrWhiteSpace(Keyword) ? null : Keyword,
                    categoryId,
                    type,
                    FromDate,
                    ToDate
                );

                Results = new ObservableCollection<Transaction>(results);

                if (Results.Count == 0)
                    ErrorMessage = "No transactions found matching your search.";
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while searching.";
            }
        }

        private void OpenTransaction()
        {
            if (SelectedTransaction == null) return;
            _navigationService.OpenTransactionWindow(_currentUser, SelectedTransaction);
        }
    }
}
