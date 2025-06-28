using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace PerEntityExample.Models;

public class DetailedFinancialRecord(
    string identifier,
    double lastPrice,
    double lastSize,
    double ask,
    double askSize,
    double bid,
    double bidSize) : SizedFinancialRecord(identifier, lastPrice, lastSize)
{
    [BsonIgnoreIfNull, BsonElement("ask")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Ask { get; private set; } = ask;

    [BsonIgnoreIfNull, BsonElement("askSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? AskSize { get; private set; } = askSize;

    [BsonIgnoreIfNull, BsonElement("bid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Bid { get; private set; } = bid;

    [BsonIgnoreIfNull, BsonElement("bidSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? BidSize { get; private set; } = bidSize;
}