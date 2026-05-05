using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SmartSaving.Commands;
using SmartSaving.Services;

namespace SmartSaving.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand OpenLoginCommand { get; }

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            OpenLoginCommand = new RelayCommand(OpenLogin);
        }

        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Please enter your first and last name.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                ErrorMessage = "Please enter a valid email address.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter a password.";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            try
            {
                var user = await _authService.RegisterAsync(Email, Password, FirstName, LastName);

                if (user == null)
                {
                    ErrorMessage = "Registration failed. Email may already be in use.";
                    return;
                }

                
                _navigationService.OpenLoginWindow();
                CloseCurrentWindow();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        private void OpenLogin()
        {
            _navigationService.OpenLoginWindow();
            CloseCurrentWindow();
        }

        private void CloseCurrentWindow()
        {
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