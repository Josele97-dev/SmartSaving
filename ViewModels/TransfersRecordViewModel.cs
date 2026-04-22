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
        public class TransferDisplayItem
        {
            public string Date { get; set; } = string.Empty;
            public string PersonName { get; set; } = string.Empty;
            public string Note { get; set; } = string.Empty;
            public decimal Amount { get; set; }

            public static TransferDisplayItem FromTransaction(Transaction t, bool isSent)
            {
                var description = t.Description ?? string.Empty;
                var separator = isSent ? " → " : " ← ";
                string name = string.Empty;
                string note = string.Empty;

                var separatorIndex = description.IndexOf(separator);
                if (separatorIndex >= 0)
                {
                    note = description.Substring(0, separatorIndex).Trim();
                    name = description.Substring(separatorIndex + separator.Length).Trim();
                }
                else
                {
                    // Handle old format "Transfer to/from [Name]"
                    var prefix = isSent ? "Transfer to " : "Transfer from ";
                    if (description.StartsWith(prefix))
                        name = description.Substring(prefix.Length).Trim();
                    else
                        note = description;
                }

                return new TransferDisplayItem
                {
                    Date = t.Date.ToString("dd/MM/yyyy"),
                    PersonName = string.IsNullOrEmpty(name) ? "Unknown" : name,
                    Note = note,
                    Amount = t.Amount
                };
            }
        }
    }
}
