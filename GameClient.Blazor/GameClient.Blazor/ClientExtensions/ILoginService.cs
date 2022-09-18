namespace GameClient.Blazor.ClientExtensions;

public interface ILoginService
{
    Task LoginUser(string userId);
}
