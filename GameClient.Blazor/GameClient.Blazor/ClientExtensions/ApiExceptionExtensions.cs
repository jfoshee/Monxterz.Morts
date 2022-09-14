using Monxterz.StatePlatform.Client;

namespace GameClient.Blazor.ClientExtensions;

public static class ApiExceptionExtensions
{
    public static string SimpleMessage(this ApiException apiException)
    {
        // Response has the complete the server-side exception message (including call stack)
        var firstLine = apiException.Response?.FirstLine() ?? apiException.Message;
        // Remove the exception type before the colon
        var parts = firstLine.Split(':', 2);
        return parts[1];
    }
}