using Microsoft.AspNetCore.SignalR.Client;

namespace GameClient.Blazor.ClientExtensions;

public class NotificationSubscriptionService
{
    private readonly IConfiguration configuration;
    private readonly IEntityCache entityCache;
    private readonly IGameStateClient gameStateClient;
    private readonly ILogger<NotificationSubscriptionService> logger;
    private HubConnection? hubConnection;

    public NotificationSubscriptionService(IConfiguration configuration,
                                           IEntityCache entityCache,
                                           IGameStateClient gameStateClient,
                                           ILogger<NotificationSubscriptionService> logger)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        this.gameStateClient = gameStateClient ?? throw new ArgumentNullException(nameof(gameStateClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Connect to the Notification Server
    /// </summary>
    public async Task Start()
    {
        var notificationsUrl = configuration["NotificationsUrl"];
        if (notificationsUrl is null)
        {
            logger.LogError("Config['NotificationsUrl'] is missing. Notifications will not work.");
            return;
        }
        hubConnection = new HubConnectionBuilder()
        .WithUrl(notificationsUrl)
            .Build();
        hubConnection.On<string>("ReceiveNotification", RefreshEntity);
        await hubConnection.StartAsync();
    }

    private async Task RefreshEntity(string id)
    {
        // TODO: Don't fetch entity if player just updated it and player owns it
        // HACK: Always fetch entity
        var entity = await gameStateClient.GetEntityAsync(id)
                     ?? throw new Exception($"Received unexpected null response for updated entity '{id}'.");
        entityCache.Set(entity);
        // TODO: Raise event to update GUI
    }
}
