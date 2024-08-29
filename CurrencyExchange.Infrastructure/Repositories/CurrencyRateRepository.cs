using CurrencyExchange.Infrastructure.DataAccess;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Repositories
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly CurrencyRateContext _context;

        public CurrencyRateRepository(CurrencyRateContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CurrencyRateEntity currencyRateEntity)
        {
            await _context.CurrencyRates.AddAsync(currencyRateEntity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<CurrencyRateEntity> currencyRateEntities)
        {
            await _context.CurrencyRates.AddRangeAsync(currencyRateEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CurrencyRateEntity>> GetAllAsync()
        {
            return await _context.CurrencyRates.ToListAsync();
        }
    }
}
