using GameClient.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddGameStateClientServices(Constants.GameMasterId, baseUrl)
                .AddBlazoredLocalStorage()
                .AddBlazoredToast()
                .AddSingleton<IEntityCache, EntityCache>()
                .AddSingleton<NotificationSubscriptionService>()
                .AddTransient<ICharacterFactory, CharacterFactory>()
                .AddTransient<ILoginService, LoginService>()
                .AddTransient<ILogoutService, LogoutService>();

var app = builder.Build();
var notificationSubscriptionService = app.Services.GetRequiredService<NotificationSubscriptionService>();
await notificationSubscriptionService.Start();
await app.RunAsync();
