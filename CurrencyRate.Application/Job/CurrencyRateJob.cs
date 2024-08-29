using Coravel.Invocable;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Domain.Constants;
using CurrencyRate.Domain.Entities;

namespace CurrencyRate.Application.Job;

public class CurrencyRateJob : IInvocable
{
    private readonly ICurrencyRateQuery _currencyRateQuery;

    public CurrencyRateJob(ICurrencyRateQuery currencyRateQuery)
    {
        _currencyRateQuery = currencyRateQuery;
    }

    public async Task Invoke()
    {
        var usdTask = _currencyRateQuery.Execute(Urls.AddressUsdPage, Selectors.SelectorForUsdRatesSheets);
        var eurTask = _currencyRateQuery.Execute(Urls.AddressEurPage, Selectors.SelectorForEurRatesSheets);
        var plnTask = _currencyRateQuery.Execute(Urls.AddressPlnPage, Selectors.SelectorForPlnRatesSheets);

        var results = await Task.WhenAll(usdTask, eurTask, plnTask);

        var allRates = results.SelectMany(r => r).ToList();

        var currencyRateEntities = CreateCurrencyRateEntity(allRates);
    }

    private List<CurrencyRateEntity> CreateCurrencyRateEntity(List<RateDto> rateDtos)
    {
        var currencyRateEntities = new List<CurrencyRateEntity>();
        foreach (var rateDto in rateDtos)
        {
            var currencyRateEntity = new CurrencyRateEntity
            {
                Id = Guid.NewGuid(),
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