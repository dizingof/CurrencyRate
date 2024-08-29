using CurrencyRate.Domain.Entities;

namespace CurrencyRate.Application.DataAccess.Repositories
{
    public interface ICurrencyRateRepository
    {
        Task AddAsync(CurrencyRateEntity currencyRateEntity);
        Task AddRangeAsync(List<CurrencyRateEntity> currencyRateEntities);
        Task<List<CurrencyRateEntity>> GetAllAsync();
    }
}
