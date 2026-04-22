using SmartSaving.Models;

namespace SmartSaving.Services
{
    public interface INavigationService
    {
        void OpenLoginWindow();
        void OpenRegisterWindow();
        void OpenMainWindow(User user);
        void OpenTransactionWindow(User user, Transaction? transaction = null);
        void OpenManageCategoriesWindow(User user);
        void OpenSearchWindow(User user);
        void OpenMonthlyTransactionsWindow(User user);
        void OpenTransferWindow(User user);
        void OpenTransfersRecordWindow(User user);
    }
}

