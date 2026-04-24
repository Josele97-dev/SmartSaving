using SmartSaving.Commands;
using SmartSaving.Events;
using SmartSaving.Models;
using SmartSaving.Repositories;
using SmartSaving.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public  class TransferViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly User _currentUser;

        public Action? CloseAction { get; set; }

        public Func<string, decimal, Task<bool>>? RequestConfirmAction { get; set; }
        public Action<string, decimal>? ShowSuccessAction { get; set; }
        public Action<string>? ShowErrorAction { get; set; }

        private ObservableCollection<User> _availableRecipients = new();
        public ObservableCollection<User> AvailableRecipients
        {
            get => _availableRecipients;
            set { _availableRecipients = value; OnPropertyChanged(); }
        }

        private User? _selectedRecipient;
        public User? SelectedRecipient
        {
            get => _selectedRecipient;
            set { _selectedRecipient = value; OnPropertyChanged(); }
        }
        private decimal _currentBalance;
        public decimal CurrentBalance
        {
            get => _currentBalance;
            set { _currentBalance = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        

        public ICommand TransferCommand { get; }

        public TransferViewModel(ITransactionService transactionService, IUserRepository userRepository, User currentUser)
        {
            _transactionService = transactionService;
            _userRepository = userRepository;
            _currentUser = currentUser;

            TransferCommand = new AsyncRelayCommand(TransferAsync);

            _ = LoadRecipientsAsync();
        }

        public void ClearForm()
        {
            SelectedRecipient = null;
            Amount = 0;
            Description = string.Empty;
            ErrorMessage = string.Empty;
        }

        private async Task LoadRecipientsAsync()
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                var others = allUsers.Where(u => u.Id != _currentUser.Id).ToList();
                AvailableRecipients = new ObservableCollection<User>(others);

                var transactions = await _transactionService.GetTransactionsAsync(_currentUser.Id);
                CurrentBalance = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount)
                               - transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            }
            catch (Exception)
            {
                ErrorMessage = "Could not load recipients.";
            }
        }

        private async Task TransferAsync()
        {
            ErrorMessage = string.Empty;

            if (SelectedRecipient == null)
            {
                ErrorMessage = "Please select a recipient.";
                return;
            }

            if (Amount <= 0)
            {
                ErrorMessage = "Amount must be greater than zero.";
                return;
            }

            if (RequestConfirmAction != null)
            {
                var confirmed = await RequestConfirmAction.Invoke(
                    $"{SelectedRecipient.FirstName} {SelectedRecipient.LastName}", Amount);
                if (!confirmed) return;
            }

            try
            {
                var recipientName = $"{SelectedRecipient.FirstName} {SelectedRecipient.LastName}";
                var amount = Amount;

                await _transactionService.TransferAsync(_currentUser, SelectedRecipient, Amount, Description);
                EventAggregator.PublishTransactionChanged();

                ShowSuccessAction?.Invoke(recipientName, amount);
                await Task.Delay(2500);
                CloseAction?.Invoke();
            }
            catch (InvalidOperationException ex)
            {
                ShowErrorAction?.Invoke(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ShowErrorAction?.Invoke(ex.Message);
            }
            catch (Exception)
            {
                ShowErrorAction?.Invoke("An unexpected error occurred during the transfer.");
            }
        }
    }
}


  
