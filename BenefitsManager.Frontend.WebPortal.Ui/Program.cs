using BenefitsManager.Common.Models;
using BenefitsManager.Frontend.WebPortal.Ui.Services;
using BenefitsManager.Frontend.WebPortal.Ui.Components;
using Microsoft.ApplicationInsights.Extensibility;
using BenefitsManager.Frontend.WebPortal.Ui;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<TelemetryConfiguration>((o) => {
    o.TelemetryInitializers.Add(new AppInsightsTelemetryInitializer());
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Dapr client
builder.Services.AddDaprClient();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5103") });
builder.Services.AddScoped<ClaimsService>();

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddConsole();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
