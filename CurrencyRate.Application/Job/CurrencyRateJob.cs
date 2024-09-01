using System.Diagnostics;
using Coravel.Invocable;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Domain.Constants;
using CurrencyRate.Domain.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CurrencyRate.Application.Job;

public class CurrencyRateJob : IInvocable
{
    private readonly ICurrencyRateQuery _currencyRateQuery;
    private readonly ICurrencyRateRepository _currencyRateRepository;
    private readonly ILogger<CurrencyRateJob> _logger;

    public CurrencyRateJob
    (
        ICurrencyRateQuery currencyRateQuery,
        ICurrencyRateRepository currencyRateRepository,
        ILogger<CurrencyRateJob> logger
    )
    {
        _currencyRateQuery = currencyRateQuery;
        _currencyRateRepository = currencyRateRepository;
        _logger = logger;
    }

    public async Task Invoke()
    {
        try
        {
            var telemetryClient = new TelemetryClient(new TelemetryConfiguration("9625215a-e7cd-41d5-845a-61d62224ef64"));

            _logger.LogInformation("Job started");
            var timestamp = DateTimeOffset.Now;
            var stopwatch = Stopwatch.StartNew();

            var usdTask = _currencyRateQuery.Execute(Urls.AddressUsdPage, Selectors.SelectorForUsdRatesSheets);
            var eurTask = _currencyRateQuery.Execute(Urls.AddressEurPage, Selectors.SelectorForEurRatesSheets);
            var plnTask = _currencyRateQuery.Execute(Urls.AddressPlnPage, Selectors.SelectorForPlnRatesSheets);

            var results = await Task.WhenAll(usdTask, eurTask, plnTask);

            var allRates = results.SelectMany(r => r).ToList();

            var currencyRateEntities = CreateCurrencyRateEntity(allRates);

            await _currencyRateRepository.AddRangeAsync(currencyRateEntities);

            stopwatch.Stop();

            var dependency = new DependencyTelemetry
            {
                Type = "Mongo",
                Name = "CurrencyRateJob",                
                Timestamp = timestamp,
                Duration = stopwatch.Elapsed,
                Success = true,
            };

            telemetryClient.TrackDependency(dependency);

            _logger.LogInformation("Job finished");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Job exception: {ex.Message}");
        }
        
    }

    private List<CurrencyRateEntity> CreateCurrencyRateEntity(List<RateDto> rateDtos)
    {
        var currencyRateEntities = new List<CurrencyRateEntity>();
        foreach (var rateDto in rateDtos)
        {
            var currencyRateEntity = new CurrencyRateEntity
            {
                _id = ObjectId.GenerateNewId(),
                CreatedDate = DateTime.Now,
                BuyRate = decimal.Parse(rateDto.BuyRate),
                SellRate = decimal.Parse(rateDto.SellRate),
                BankName = rateDto.BankName,
                CurrencyCode = rateDto.CurrencyCode
            };
            currencyRateEntities.Add(currencyRateEntity);
        }

        return currencyRateEntities;
    }
}