using System.Diagnostics;
using Coravel.Invocable;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Domain.Constants;
using CurrencyRate.Domain.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CurrencyRate.Application.Job;

public class CurrencyRateJob : IInvocable
{
    private readonly ICurrencyRateQuery _currencyRateQuery;
    private readonly ICurrencyRateRepository _currencyRateRepository;
    private readonly ILogger<CurrencyRateJob> _logger;
    private readonly TelemetryClient _telemetryClient;

    public CurrencyRateJob
    (
        ICurrencyRateQuery currencyRateQuery,
        ICurrencyRateRepository currencyRateRepository,
        ILogger<CurrencyRateJob> logger,
        TelemetryClient telemetryClient
    )
    {
        _currencyRateQuery = currencyRateQuery;
        _currencyRateRepository = currencyRateRepository;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task Invoke()
    {
        try
        {
            _logger.LogInformation("Job started");

            var allRates = GetRateFromExternalResource();
            var currencyRateEntities = CreateCurrencyRateEntity(await allRates);

            var timestamp = DateTimeOffset.Now;
            var stopwatch = Stopwatch.StartNew();

            await _currencyRateRepository.AddRangeAsync(currencyRateEntities);

            stopwatch.Stop();

            var dependency = new DependencyTelemetry
            {
                Type = "Database",
                Name = "Mongo",
                Timestamp = timestamp,
                Duration = stopwatch.Elapsed,
                Success = true,
                Data = nameof(CurrencyRateJob)
            };

            _telemetryClient.TrackDependency(dependency);

            _logger.LogInformation("Job finished");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Job exception: {ex.Message}");
            var exceptionTelemetry = new ExceptionTelemetry(ex)
            {
                SeverityLevel = SeverityLevel.Error,
                Timestamp = DateTimeOffset.Now
            };
            _telemetryClient.TrackException(exceptionTelemetry);
        }

    }

    private async Task<List<RateDto>> GetRateFromExternalResource()
    {
        var usdTask = _currencyRateQuery.Execute(Urls.AddressUsdPage, Selectors.SelectorForUsdRatesSheets);
        var eurTask = _currencyRateQuery.Execute(Urls.AddressEurPage, Selectors.SelectorForEurRatesSheets);
        var plnTask = _currencyRateQuery.Execute(Urls.AddressPlnPage, Selectors.SelectorForPlnRatesSheets);

        var results = await Task.WhenAll(usdTask, eurTask, plnTask);

        var allRates = results.SelectMany(r => r).ToList();
        return allRates;
    }

    private List<CurrencyRateEntity> CreateCurrencyRateEntity(List<RateDto> rateDtos)
    {
        var currencyRateEntities = new List<CurrencyRateEntity>();
        foreach (var rateDto in rateDtos)
        {
            var currencyRateEntity = new CurrencyRateEntity
            {
                Id = ObjectId.GenerateNewId(),
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