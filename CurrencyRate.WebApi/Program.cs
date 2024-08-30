using Coravel;
using CurrencyExchange.Infrastructure.DataAccess;
using CurrencyExchange.Infrastructure.DataAccess.Query;
using CurrencyExchange.Infrastructure.Repositories;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Application.Job;
using CurrencyRate.WebApi.RequestHandle;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();

builder.Logging.AddAzureWebAppDiagnostics();
builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScheduler();
builder.Services.AddScoped<CurrencyRateJob>();
builder.Services.AddScoped<ICurrencyRateQuery, CurrencyRateQuery>();
builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<CurrencyRateContext>();
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
    scheduler.Schedule<CurrencyRateJob>().Hourly();
});

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
