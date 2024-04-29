using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

using MudBlazor;
using MudBlazor.Services;

using Reader.Modules.Logging;
using Reader.Modules.Middleware;
using Reader.Data.Storage;
using Reader.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Config/appsettings.json");
builder.Configuration.AddJsonFile("Config/appsettings.Development.json");

// CONFIG

var clArgs = Environment.GetCommandLineArgs();

AppConfig config;

if (clArgs.Length >= 2)
{
    config = AppConfig.GetFromJsonFile(clArgs[1]);
} else
{
    config = AppConfig.GetFromJsonFile("Config/AppConfig_Default.json");
}

// SERVICES SETUP

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMudServices();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 8000;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// API

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new RoutePrefixConvention("api"));
}).AddControllersAsServices();

builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000; // 100 MB
});

// CONSTR

//#if DEBUG
//string ConnectionString = builder.Configuration["Database:ConnectionStringTesting"]!;
//#else
//string ConnectionString  = builder.Configuration["Database:ConnectionStringProduction"]!;
//#endif

// DB TESTING 

#if DEBUG
Log.Warning("Test");
#endif

// MODULES

builder.Services.AddSingleton<Constants>();
builder.Services.AddSingleton<BaseUIStorage>();
builder.Services.AddScoped<LoggingMiddleware>();
builder.Services.AddScoped<AppConfig>();

// prevents 404 when switching environments - see https://github.com/MudBlazor/Templates/commit/62e13c61058b419b8957f7d19f38c69a70ef50e6
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

var app = builder.Build();

// MIDDLEWARE

app.UseMiddleware<LoggingMiddleware>();


// HSTS

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Base/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "static")),
    RequestPath = "/static"
});

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/Base/_Host");

app.Run();
