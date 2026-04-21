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

        private async Task LoadRecipientsAsync()
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                var others = allUsers.Where(u => u.Id != _currentUser.Id).ToList();
                AvailableRecipients = new ObservableCollection<User>(others);
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

            try
            {
                await _transactionService.TransferAsync(_currentUser, SelectedRecipient, Amount, Description);
                EventAggregator.PublishTransactionChanged();
                CloseAction?.Invoke();
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
                ErrorMessage = "An unexpected error occurred during the transfer.";
            }
        }
    }
}


  
