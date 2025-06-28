using MongoDB.Bson;
using MongoDB.Driver;
using PerEntityExample.Models;

namespace PerEntityExample.Writers;

/// <summary>
/// Key-based mongo writer
/// </summary>
public class MongoWriter<TMessage> : IWriter<TMessage> where TMessage : IFinancialRecord
{
    private readonly IMongoDatabase _database;
    private readonly Dictionary<string, IMongoCollection<BsonDocument>> _collections = new();

    public MongoWriter(string database, string connectionString)
    {
        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.MaxConnectionPoolSize = 250;
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(5);
        var mongoClient = new MongoClient(settings);

        _database = mongoClient.GetDatabase(database);
    }

    private readonly InsertManyOptions _insertingOptions = new() { BypassDocumentValidation = true, Comment = null, IsOrdered = false };

    private IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
        if (_collections.TryGetValue(collectionName, out var collection))
            return collection;

        collection = _database.GetCollection<BsonDocument>(collectionName);
        _collections[collectionName] = collection;

        return collection;
    }

    public async Task WriteAsync(Dictionary<string, List<TMessage>> data, CancellationToken cancellationToken = default)
    {
        var writeOperations = from groupedTick in data
                              let collectionName = groupedTick.Key
                              let collection = GetCollection(collectionName)
                              let documents = groupedTick.Value.Select(x => x.ToBsonDocument()).ToList()
                              select new WriteOperation(collection, documents);

        var tasks = writeOperations.Select(async op => await op.WriteAsync(_insertingOptions, cancellationToken)).ToArray();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task WriteAsync(string key, List<TMessage> data, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(key);
        var documents = data.Select(x => x.ToBsonDocument()).ToList();
        await collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task WriteAsync(string key, TMessage data, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(key);
        var documents = data.ToBsonDocument();
        await collection.InsertOneAsync(documents, cancellationToken: cancellationToken);
    }
}

public class WriteOperation(IMongoCollection<BsonDocument> collection, IEnumerable<BsonDocument> documents)
{
    private IMongoCollection<BsonDocument> Collection { get; } = collection;
    private IEnumerable<BsonDocument> Documents { get; } = documents.Select(document =>
    {
        // remove discriminator
        document.Remove("_t");
        return document;
    });

    public async Task WriteAsync(InsertManyOptions options, CancellationToken cancellationToken) =>
        await Collection.InsertManyAsync(Documents, options, cancellationToken);
}