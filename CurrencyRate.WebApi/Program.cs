using Coravel;
using CurrencyExchange.Infrastructure.DataAccess;
using CurrencyExchange.Infrastructure.DataAccess.Query;
using CurrencyExchange.Infrastructure.Repositories;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Application.Job;
using CurrencyRate.Application.Services;
using CurrencyRate.WebApi.RequestHandle;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;


try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    // 1. Logs settings
    builder.Logging.ClearProviders();
    builder.Logging.AddAzureWebAppDiagnostics();
    builder.Logging.AddConsole();
    builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);

    // 2. Regestration application insight
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddSingleton<TelemetryClient>();

    // 3. Db context setting for MongoDb Connect
    builder.Services.AddDbContext<CurrencyRateContext>(options =>
    {
        var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        var mongoDbName = configuration["MongoSettings:DataBaseName"];
        options.UseMongoDB(mongoConnectionString, mongoDbName);
    });

    // 4. Regestration DI
    builder.Services.AddScoped<CurrencyRateContext>();
    builder.Services.AddScoped<CurrencyRateJob>();
    builder.Services.AddScoped<ICurrencyRateQuery, CurrencyRateQuery>();
    builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
    builder.Services.AddTransient<ICurrencyRateService, CurrencyRateService>();

    // 5. Setting Controllers, Swager and other
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScheduler();
    builder.Services.AddHttpClient();
    builder.Services.AddExceptionHandler<ExceptionHandler>();

    var app = builder.Build();

    // 6. Http request settings pipeline
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // 7. Settings scheduler
    app.Services.UseScheduler(scheduler =>
    {
        scheduler.Schedule<CurrencyRateJob>().Cron("3 * * * *");
    });

    // 8. Setting differents midlware
    app.UseExceptionHandler(_ => { });
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // 9. Run app
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error run: {ex.Source}");
    throw;
}