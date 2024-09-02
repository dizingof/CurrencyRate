using CurrencyRate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.DataAccess
{
    public class CurrencyRateContext : DbContext
    {
        public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMongoDB("mongodb+srv://romannep1989:kOE3i30iZnYPd5zk@cluster0.udc4owj.mongodb.net/", "CurrencyRateEntity");
        }
    }
}
