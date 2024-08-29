using CurrencyExchange.Infrastructure.DataAccess;
using CurrencyRate.Application.DataAccess.Repositories;
using CurrencyRate.Domain.Entities;

namespace CurrencyExchange.Infrastructure.Repositories
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly CurrencyRateContext _context;

        public CurrencyRateRepository(CurrencyRateContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CurrencyRateEntity entity)
        {
            await _context.CurrencyRates.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CurrencyRateEntity>> GetAllAsync()
        {
            return await _context.CurrencyRates.ToListAsync();
        }
    }
}
