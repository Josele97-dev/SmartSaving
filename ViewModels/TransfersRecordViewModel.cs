using SmartSaving.Events;
using SmartSaving.Models;
using SmartSaving.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSaving.ViewModels
{
    public  class TransfersRecordViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly User _currentUser;

        public string MonthTitle => $"{DateTime.Now:MMMM yyyy}";

        private ObservableCollection<TransferDisplayItem> _sentTransfers = new();
        public ObservableCollection<TransferDisplayItem> SentTransfers
        {
            get => _sentTransfers;
            set { _sentTransfers = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoSent)); }
        }

        private ObservableCollection<TransferDisplayItem> _receivedTransfers = new();
        public ObservableCollection<TransferDisplayItem> ReceivedTransfers
        {
            get => _receivedTransfers;
            set { _receivedTransfers = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoReceived)); }
        }

        public bool HasNoSent => _sentTransfers == null || !_sentTransfers.Any();
        public bool HasNoReceived => _receivedTransfers == null || !_receivedTransfers.Any();
        public TransfersRecordViewModel(ITransactionService transactionService, User user)
        {
            _transactionService = transactionService;
            _currentUser = user;

            EventAggregator.TransactionChanged += async () => await LoadDataAsync();
            _ = LoadDataAsync();
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

                SentTransfers = new ObservableCollection<TransferDisplayItem>(
                thisMonth.Where(t => t.Category?.Title == "Transfer Out")
                         .OrderByDescending(t => t.Date)
                         .Select(t => TransferDisplayItem.FromTransaction(t, true)));

                ReceivedTransfers = new ObservableCollection<TransferDisplayItem>(
                thisMonth.Where(t => t.Category?.Title == "Transfer In")
                         .OrderByDescending(t => t.Date)
                         .Select(t => TransferDisplayItem.FromTransaction(t, false)));
            }
            catch (Exception)
            {
                // Handle silently
            }
        }
        
        
    }
}
