using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SmartSaving.Commands;
using SmartSaving.Services;

namespace SmartSaving.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            LoginCommand = new AsyncRelayCommand(LoginAsync);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                return;
            }

            try
            {
                var user = await _authService.LoginAsync(Email, Password);

                if (user == null)
                {
                    ErrorMessage = "Incorrect email or password.";
                    return;
                }

                _navigationService.OpenMainWindow(user);

                // Close the current login window
                CloseCurrentWindow();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        private void OpenRegister()
        {
            _navigationService.OpenRegisterWindow();
        }

        private void CloseCurrentWindow()
        {
            // Finds and closes the window that has this ViewModel as its DataContext
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}