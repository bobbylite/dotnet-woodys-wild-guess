using dotnet.woodyswildguess.Components;
using dotnet.woodyswildguess.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration
builder.AddTwitterConfiguration();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Twitter Service
builder.AddTwitterServices();

// Add Hugging Face AI and ML Services
builder.Services.AddHuggingFaceServices();

// Add Access Control
builder.AddAccessControl();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add Api Controllers
builder.Services.AddControllers();

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

// Map the chat hub
app.MapChatHub();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(dotnet.woodyswildguess.Client._Imports).Assembly);

app.MapGroup("/authentication")
    .MapLoginAndLogout();

app.Run();
