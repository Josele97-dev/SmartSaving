using SmartSaving.Models;
using SmartSaving.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace SmartSaving.ViewModels
{
    public class TransferNotificationViewModel : BaseViewModel
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly int _accountId;

        public ObservableCollection<TransferDisplayItem> UnreadTransfers { get; set; } = new();

        public string NotificationMessage => UnreadTransfers.Count == 1
            ? "You received 1 new transfer while you were away."
            : $"You received {UnreadTransfers.Count} new transfers while you were away.";

        public TransferNotificationViewModel(ITransactionRepository transactionRepository, int accountId, List<Transaction> unreadTransfers)
        {
            _transactionRepository = transactionRepository;
            _accountId = accountId;
            UnreadTransfers = new ObservableCollection<TransferDisplayItem>(
                unreadTransfers.Select(t => TransferDisplayItem.FromTransaction(t, false)));
        }

        public async Task MarkAsReadAsync()
        {
            await _transactionRepository.MarkTransfersAsReadAsync(_accountId);
        }
    }
}
