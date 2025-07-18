using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp;
using WebApp.Service;
using WebApp.Service.AuthServices;
using WebApp.Service.CalendarServices;
using WebApp.Service.FineServices;
using WebApp.Service.HighlightServices;
using WebApp.Service.PointServices;
using WebApp.Service.RuleServices;
using WebApp.Service.UploadServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5176/") });
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFineService, FineServiceMock>();
builder.Services.AddScoped<IHighlightService, HighlightService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IPointService, PointService>();

await builder.Build().RunAsync();