using Blazored.LocalStorage;
using GameClient.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddGameStateClientServices(Constants.GameMasterId, baseUrl)
                .AddBlazoredLocalStorage()
                .AddSingleton<IEntityCache, EntityCache>()
                .AddTransient<ICharacterFactory, CharacterFactory>();

await builder.Build().RunAsync();
