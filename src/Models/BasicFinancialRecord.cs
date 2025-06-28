using MongoDB.Bson.Serialization.Attributes;

namespace PerEntityExample.Models;

public class BasicFinancialRecord(string identifier, double lastPrice) : IFinancialRecord
{
    [BsonIgnore]
    public string Key { get; } = identifier;

    [BsonElement("price")]
    public double LastPrice { get; private set; } = lastPrice;
    [BsonElement("lastTradeAt")]
    public DateTime LastTradeAt { get; private set; } = DateTime.UtcNow;
}