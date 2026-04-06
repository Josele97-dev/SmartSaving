using SmartSaving.Models;

namespace SmartSaving.Services
{
    public interface INavigationService
    {
        void OpenLoginWindow();
        void OpenRegisterWindow();
        void OpenMainWindow(User user);
        void OpenTransactionWindow(User user);
    }
}

