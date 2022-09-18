using GameClient.Blazor.Components;

namespace GameClient.Blazor.ClientExtensions;

public static class ToastServiceExtensions
{
    public static void ShowInfoRpg(this IToastService toastService, string message)
    {
        var toastParameters = new ToastParameters();
        toastParameters.Add(nameof(RpgToast.Title), message);
        toastService.ShowToast<RpgToast>(toastParameters);
    }
    public static void ShowErrorRpg(this IToastService toastService, string message)
    {
        // TODO: Error styling
        ShowInfoRpg(toastService, message);
    }
}
