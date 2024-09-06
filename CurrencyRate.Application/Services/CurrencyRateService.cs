using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Domain.Entities;

namespace CurrencyRate.Application.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly ICurrencyRateRepository _currencyRateRepository;

        public CurrencyRateService(ICurrencyRateRepository currencyRateRepository)
        {
            _currencyRateRepository = currencyRateRepository;
        }

        public async Task<List<CurrencyRateEntity>> GetLatestRateByCodeAsync(string currencyCode)
        {
            var today = DateTime.UtcNow.Date;         

            var todayRates = await _currencyRateRepository.GetRateByDateAsync(today);

            if (todayRates.Count != 0) 
            {
                var lastUpdatedTime = todayRates.Max(rate => rate.CreatedDate);

                var latestUpdatedRates = todayRates.Where(x => x.CreatedDate == lastUpdatedTime)
                                                   .Where(x => x.CurrencyCode == currencyCode)
                                                   .ToList();

                return latestUpdatedRates; 
            }
            return todayRates;
        }
    }
}
