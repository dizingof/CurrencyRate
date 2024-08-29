using CurrencyRate.Domain.Entities;

namespace CurrencyRate.Application.DataAccess.Query
{
    public interface ICurrencyRateQuery
    {
        Task<List<RateDto>> Execute(string address, string selector);
    }
}
