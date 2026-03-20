using SmartSaving.Views;

public class NavigationService : INavigationService
{
    private readonly IAuthService _authService;

    public NavigationService(IAuthService authService)
    {
        _authService = authService;
    }

    public void OpenMainWindow(User user)
    {
        var vm = new MainViewModel(user);
        var window = new MainWindow(vm);
        window.Show();
    }

    public void OpenLoginWindow()
    {
        var vm = new LoginViewModel(_authService);
        var window = new LoginWindow(vm);
        window.Show();
    }
}