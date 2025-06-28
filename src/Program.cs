using PerEntityExample.Models;
using PerEntityExample.Writers;

var indexes = new[] { "XU30", "XU100", "XU500" };
var stocks = new[] { "EKGYO", "GARAN", "ISMEN" };

const string target = "financialRecords";
var writers = new List<IWriter<IFinancialRecord>>
{
    new FileWriter<IFinancialRecord>(target),
    new MongoWriter<IFinancialRecord>(target, "mongodb://localhost:27017")
};

const int batchSize = 100;
while (true)
{
    var buffer = new List<IFinancialRecord>(batchSize);

    var recordStreaming = GetFinancialRecordStreamingAsync();
    await foreach (var record in recordStreaming)
    {
        buffer.Add(record);

        if (buffer.Count >= batchSize)
        {
            var groupedBuffer = buffer
                .GroupBy(r => r.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            var writeTasks = writers.Select(writer => writer.WriteAsync(groupedBuffer)).ToArray();
            try
            {
                await Task.WhenAll(writeTasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writing records: {e.Message}");
            }
            finally
            {
                buffer.Clear();
                groupedBuffer.Clear();
            }
        }
    }
}

async IAsyncEnumerable<IFinancialRecord> GetFinancialRecordStreamingAsync()
{
    var random = new Random();

    var prices = new Dictionary<string, double>();
    foreach (var key in indexes.Concat(stocks))
        prices[key] = 10 + random.NextDouble() * 10;

    while (true)
    {
        string key;
        var isBasic = false;
        if (random.Next(2) == 0)
        {
            key = stocks[random.Next(stocks.Length)];
        }
        else
        {
            key = indexes[random.Next(indexes.Length)];
            isBasic = true;
        }

        var prevPrice = prices[key];

        var delta = random.NextDouble() * 0.2 - 0.1;
        var price = Math.Max(0, prevPrice + delta);

        prices[key] = price;

        if (isBasic)
        {
            yield return new BasicFinancialRecord(key, price);
        }
        else
        {
            var size = random.Next(1, 500);
            var ask = price + random.NextDouble() * 0.1;
            var askSize = random.Next(1, 500);
            var bid = price - random.NextDouble() * 0.1;
            var bidSize = random.Next(1, 500);

            yield return new DetailedFinancialRecord(key, price, size, ask, askSize, bid, bidSize);
        }

        await Task.Delay(50);
    }
}
