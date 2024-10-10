using dotnet.woodyswildguess.Client.Pages;
using dotnet.woodyswildguess.Components;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Services.Twitter;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration
builder.AddTwitterConfiguration();

// Add Access Control
builder.AddAccessControl();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add Api Controllers
builder.Services.AddControllers();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Twitter Service
builder.AddTwitterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, 
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(dotnet.woodyswildguess.Client._Imports).Assembly);

app.MapGroup("/authentication")
    .MapLoginAndLogout();

app.MapControllers();

app.Run();
