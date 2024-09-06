using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CurrencyRate.Domain.Entities;

public class CurrencyRateEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string CurrencyCode { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string BankName { get; set; }
}