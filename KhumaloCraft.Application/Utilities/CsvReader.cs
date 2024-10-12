using CsvHelper.Configuration;

namespace KhumaloCraft.Application.Utilities;

/// <summary>
/// Provides a default reader for CSV files.
/// </summary>
public sealed class CsvReader : IDisposable
{
    private readonly CsvHelper.CsvReader _innerReader;
    public bool HasDelimiterRow { get; private set; }

    /// <summary>
    /// Creates a new CSV reader using the given <see cref="TextReader" />.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="configure">An <see cred="Action"> which may be used to further configure <see cref="IReaderConfiguration"/>.</param>
    /// <param name="leaveOpen"><c>true</c> to leave the <see cref="TextReader"/> open after the <see cref="CsvReader"/> object is disposed, otherwise <c>false</c>.</param>
    public CsvReader(TextReader reader, Action<IReaderConfiguration> configure = null, bool leaveOpen = false)
    {
        var config = CsvConfig.DefaultConfiguration;
        config.ShouldSkipRecord = records =>
        {
            var isDelimiterRow = records.Length == 1 && records[0] == "sep=,";
            HasDelimiterRow |= isDelimiterRow;
            return isDelimiterRow;
        };
        config.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);

        _innerReader = new CsvHelper.CsvReader(reader, config, leaveOpen);

        if (configure is not null)
        {
            configure(_innerReader.Configuration);
        }
    }

    /// <summary>
    /// Reads the header of the CSV file.
    /// </summary>
    /// <typeparam name="TRecord">The <see cref="Type"/> of the record.</typeparam>
    /// <throws>Throws a <see cref="CsvHelper.HeaderValidationException"/> if the header is invalid.</throws>
    public void ReadHeader<TRecord>()
    {
        _innerReader.Read();
        _innerReader.ReadHeader();
        _innerReader.ValidateHeader<TRecord>();
    }

    /// <summary>
    /// Reads the header of the CSV file.
    /// </summary>
    /// <typeparam name="TRecord">The <see cref="Type"/> of the record.</typeparam>
    /// <throws>Throws a <see cref="CsvHelper.HeaderValidationException"/> if the header is invalid.</throws>
    public async Task ReadHeaderAsync<TRecord>()
    {
        await _innerReader.ReadAsync();
        _innerReader.ReadHeader();
        _innerReader.ValidateHeader<TRecord>();
    }

    /// <summary>
    /// Gets all the records in the CSV file and converts each to <see cref="System.Type" /> TRecord.
    /// </summary>
    /// <typeparam name="TRecord">The <see cref="Type"/> of the record.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}" /> of records.</returns>
    public IEnumerable<TRecord> GetRecords<TRecord>()
    {
        return _innerReader.GetRecords<TRecord>();
    }

    /// <summary>
    /// Gets all the records in the CSV file and converts each to <see cref="System.Type" /> TRecord.
    /// </summary>
    /// <typeparam name="TRecord">The <see cref="Type"/> of the record.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}" /> of records.</returns>
    public IAsyncEnumerable<TRecord> GetRecordsAsync<TRecord>()
    {
        return _innerReader.GetRecordsAsync<TRecord>();
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
            _innerReader.Dispose();
        }
    }
}