using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWassm;
using BlazorWassm.StateContainer;
using HttpClients.ClientInterfaces;
using HttpClients.Implementations;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7023")});
builder.Services.AddScoped<IUserService, UserHttpClient>();
builder.Services.AddScoped<ITodoService, TodoHttpClient>();


await builder.Build().RunAsync();