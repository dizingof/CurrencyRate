using CurrencyRate.Domain.Entities;

namespace CurrencyRate.Application.Services
{
    public interface ICurrencyRateService
    {
        Task<List<CurrencyRateEntity>> GetLatestRateByCodeAsync(string currencyCode);
    }
}
