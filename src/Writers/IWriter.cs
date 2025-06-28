using PerEntityExample.Models;

namespace PerEntityExample.Writers;

/// <summary>
/// Key-based writer interface
/// </summary>
public interface IWriter<TMessage> where TMessage : IFinancialRecord
{
    /// <summary>
    /// Writes multiple groups of messages to their respective destinations, tables, collections in parallel.
    /// </summary>
    Task WriteAsync(Dictionary<string, List<TMessage>> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a batch of messages to the specified destination, table, collection.
    /// </summary>
    Task WriteAsync(string key, List<TMessage> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a single message to the specified destination, table, collection.
    /// </summary>
    Task WriteAsync(string key, TMessage data, CancellationToken cancellationToken = default);
}