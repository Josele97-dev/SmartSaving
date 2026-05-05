using SmartSaving.Commands;
using SmartSaving.Events;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using System;
using System.Linq;
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
        private bool _searchStartsWith = true;
        public bool SearchStartsWith
        {
            get => _searchStartsWith;
            set { _searchStartsWith = value; OnPropertyChanged(); }
        }

        private bool _searchContains = false;
        public bool SearchContains
        {
            get => _searchContains;
            set { _searchContains = value; OnPropertyChanged(); }
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

        public Action? CloseAction { get; set; }
        public Action? ReopenAction { get; set; }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private int _openTransactionCount = 0;

        

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
            set { _results = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoResults)); }
        }
        public bool HasNoResults => _results == null || !_results.Any();

        private Transaction? _selectedTransaction;
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }


        

        public ICommand SearchCommand { get; }

        public ICommand OpenTransactionCommand { get; }

        public SearchViewModel(ITransactionRepository transactionRepository, INavigationService navigationService, User user)
        {
            _transactionRepository = transactionRepository;
            _navigationService = navigationService;
            _currentUser = user;

            SearchCommand = new AsyncRelayCommand(SearchAsync);
            OpenTransactionCommand = new RelayCommand(OpenTransaction);

            EventAggregator.TransactionWindowClosed += OnTransactionWindowClosed;
            
            LoadCategories();
        }

        private void LoadCategories()
        {
            var defaultAccount = _currentUser.Accounts?.FirstOrDefault();
            if (defaultAccount?.Categories != null)
            {
                
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
                SearchContains,
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

            _openTransactionCount++;

            _navigationService.OpenTransactionWindow(_currentUser, SelectedTransaction);

            if (_openTransactionCount >= 3)
            {
                CloseAction?.Invoke();
            }
        }

        private void OnTransactionWindowClosed()
        {
            if (_openTransactionCount > 0)
                _openTransactionCount--;

            
            if (_openTransactionCount < 3)
                ReopenAction?.Invoke();
        }
    }
}
