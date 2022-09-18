namespace GameClient.Blazor.ClientExtensions;

public class LoginService : ILoginService
{
    private readonly IChangeUserService changeUserService;
    private readonly NavigationManager navigationManager;
    private readonly ILocalStorageService localStorage;
    private readonly IGameBearerTokenProvider gameTokenProvider;
    private readonly IGameStateClient gameStateClient;

    public LoginService(IChangeUserService changeUserService,
                        NavigationManager navigationManager,
                        ILocalStorageService localStorage,
                        IGameBearerTokenProvider gameTokenProvider,
                        IGameStateClient gameStateClient)
    {
        this.changeUserService = changeUserService ?? throw new ArgumentNullException(nameof(changeUserService));
        this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        this.localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        this.gameTokenProvider = gameTokenProvider ?? throw new ArgumentNullException(nameof(gameTokenProvider));
        this.gameStateClient = gameStateClient ?? throw new ArgumentNullException(nameof(gameStateClient));
    }

    public async Task LoginUser(string userId)
    {
        if (userId is null)
            throw new ArgumentNullException(nameof(userId));
        userId = userId.Trim();
        await changeUserService.ChangeUserAsync(userId);
        var response = await gameStateClient.GetAuthTestAsync();
        if (response?.IsAuthenticated == true && response?.UserId == userId)
        {
            // Save JWT Token
            // TODO: Break direct dependence on IGameBearerTokenProvider
            await localStorage.SetItemAsync("Token", gameTokenProvider.BearerToken);
            await localStorage.SetItemAsync("UserID", gameTokenProvider.UserId);
            navigationManager.NavigateTo("/");
        }
        else
            throw new Exception("The authentication test failed. The user could not be authenticated.");
    }
}
