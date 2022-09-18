namespace GameClient.Blazor.ClientExtensions;

public class LogoutService : ILogoutService
{
    private readonly IChangeUserService changeUserService;
    private readonly NavigationManager navigationManager;
    private readonly ILocalStorageService localStorage;

    public LogoutService(IChangeUserService changeUserService, NavigationManager navigationManager, ILocalStorageService localStorage)
    {
        this.changeUserService = changeUserService ?? throw new ArgumentNullException(nameof(changeUserService));
        this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        this.localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    public async Task Logout()
    {
        await changeUserService.ChangeUserAsync(null);
        await localStorage.RemoveItemAsync("Token");
        await localStorage.RemoveItemAsync("UserID");
        navigationManager.NavigateTo("/login");
    }
}
