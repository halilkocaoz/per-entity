using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace PerEntityExample.Models;

public class SizedFinancialRecord(string identifier, double lastPrice, double lastSize) : BasicFinancialRecord(identifier, lastPrice)
{
    [BsonIgnoreIfNull, BsonElement("lastSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? LastSize { get; private set; } = lastSize;
}