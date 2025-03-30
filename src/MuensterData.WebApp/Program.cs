using MuensterData.Domain;
using MuensterData.Infrastructure;
using MuensterData.WebApp.Localization;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddDomain();
builder.Services.AddInfrastructure();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddSingleton<ISyncfusionStringLocalizer, SyncfusionLocalizer>();

SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SfLicenseKey"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRequestLocalization("de-DE");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
