namespace CurrencyRate.Domain.Entities;

public class RateDto
{
    public string BankName { get; set; }
    public string BuyRate { get; set; }
    public string SellRate { get; set; }
    public string CurrencyCode { get; set; }
}