using Coravel;
using CurrencyExchange.Infrastructure.DataAccess;
using CurrencyExchange.Infrastructure.DataAccess.Query;
using CurrencyExchange.Infrastructure.Repositories;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Application.Job;
using CurrencyRate.WebApi.RequestHandle;
using Microsoft.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

builder.Logging.ClearProviders();

builder.Logging.AddAzureWebAppDiagnostics();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);

builder.Services.AddSingleton<CurrencyRateContext>();
builder.Services.AddSingleton<TelemetryClient>();
builder.Services.AddScoped<CurrencyRateJob>();
builder.Services.AddScoped<ICurrencyRateQuery, CurrencyRateQuery>();
builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScheduler();
builder.Services.AddHttpClient();
builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<CurrencyRateJob>().EverySeconds(10);
});

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
