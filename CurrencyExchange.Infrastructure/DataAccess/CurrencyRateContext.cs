using CurrencyRate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.DataAccess
{
    public class CurrencyRateContext : DbContext
    {
        public CurrencyRateContext(DbContextOptions<CurrencyRateContext> options)
        : base(options)
        {
        }


        public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }        
    }
}
