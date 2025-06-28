using System.Text.Json;
using PerEntityExample.Models;

namespace PerEntityExample.Writers;

/// <summary>
/// Key-based file writer
/// </summary>
public class FileWriter<TMessage> : IWriter<TMessage> where TMessage : IFinancialRecord
{
    private readonly string _baseDirectory;

    public FileWriter(string baseDirectory)
    {
        _baseDirectory = baseDirectory;

        var baseDirectoryExist = Path.Exists(_baseDirectory);
        if (baseDirectoryExist is false)
            Directory.CreateDirectory(_baseDirectory);
    }

    public async Task WriteAsync(Dictionary<string, List<TMessage>> data, CancellationToken cancellationToken = default)
    {
        var tasks = data.Select(kvp => WriteAsync(kvp.Key, kvp.Value, cancellationToken));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task WriteAsync(string key, List<TMessage> data, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_baseDirectory, $"{key}.json");

        await using var stream = new FileStream(
            filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        await using var writer = new StreamWriter(stream);

        foreach (var item in data)
        {
            var json = JsonSerializer.Serialize(item, item.GetType());
            var jsonAsMemory = json.AsMemory();
            await writer.WriteLineAsync(jsonAsMemory, cancellationToken);
        }
    }

    public async Task WriteAsync(string key, TMessage data, CancellationToken cancellationToken = default)
    {
        await WriteAsync(key, [data], cancellationToken).ConfigureAwait(false);
    }
}