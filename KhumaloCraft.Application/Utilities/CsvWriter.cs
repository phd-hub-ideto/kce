using CsvHelper.Configuration;

namespace KhumaloCraft.Application.Utilities;

/// <summary>
/// Provides a default writer for CSV files.
/// </summary>
public sealed class CsvWriter : IDisposable
{
    private readonly CsvHelper.CsvWriter _innerWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvWriter"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="configure">An <see cref="Action"> which may be used to further configure <see cref="IWriterConfiguration"/>.</param>
    /// <param name="leaveOpen"><c>true</c> to leave the <see cref="TextWriter"/> open after the <see cref="CsvWriter"/> object is disposed, otherwise <c>false</c>.</param>
    public CsvWriter(TextWriter writer, Action<IWriterConfiguration> configure = null, bool leaveOpen = false)
    {
        _innerWriter = new CsvHelper.CsvWriter(writer, CsvConfig.DefaultConfiguration, leaveOpen);
        if (configure is not null)
        {
            configure(_innerWriter.Configuration);
        }
    }

    /// <summary>
    /// Writes the records to the CSV file. Includes a delimiter row and the header.
    /// </summary>
    /// <typeparam name="TRecord">Record type.</typeparam>
    /// <param name="records">The records to write.</param>
    public void Write<TRecord>(IEnumerable<TRecord> records)
    {
        WriteAsync(records).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Writes the records to the CSV file. Includes a delimiter row and the header.
    /// </summary>
    /// <typeparam name="TRecord">Record type.</typeparam>
    /// <param name="records">The records to write.</param>
    public async Task WriteAsync<TRecord>(IEnumerable<TRecord> records)
    {
        _innerWriter.WriteField($"sep={CsvConfig.DefaultConfiguration.Delimiter}");
        await _innerWriter.NextRecordAsync();
        _innerWriter.WriteHeader<TRecord>();
        await _innerWriter.NextRecordAsync();
        await _innerWriter.WriteRecordsAsync(records);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">Indicates if the object is being disposed.</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerWriter.Dispose();
        }
    }
}