using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartSaving.Commands;
using SmartSaving.Models;
using SmartSaving.Repositories;

namespace SmartSaving.ViewModels
{
    public class ManageCategoriesViewModel : BaseViewModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly User _currentUser;

        private ObservableCollection<Category> _expenseCategories = new();
        public ObservableCollection<Category> ExpenseCategories
        {
            get => _expenseCategories;
            set { _expenseCategories = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public ManageCategoriesViewModel(ICategoryRepository categoryRepository, User user)
        {
            _categoryRepository = categoryRepository;
            _currentUser = user;

            SaveCommand = new AsyncRelayCommand(SaveAsync);

            _ = LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var defaultAccount = _currentUser.Accounts?.FirstOrDefault();
            if (defaultAccount == null) return;

            var categories = await _categoryRepository.GetByAccountIdAsync(defaultAccount.Id);
            var expenseOnly = categories
                .Where(c => c.Type == TransactionType.Expense)
                .ToList();

            // Also load expense categories without a limit set
            var allCategories = await _categoryRepository.GetAllByAccountIdAsync(defaultAccount.Id);
            var allExpense = allCategories
                .Where(c => c.Type == TransactionType.Expense)
                .ToList();

            ExpenseCategories = new ObservableCollection<Category>(allExpense);
        }

        private async Task SaveAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                foreach (var category in ExpenseCategories)
                {
                    await _categoryRepository.UpdateAsync(category);
                }
                SuccessMessage = "Budget limits saved successfully!";
            }
            catch (System.Exception)
            {
                ErrorMessage = "An error occurred while saving.";
            }
        }
    }
}

