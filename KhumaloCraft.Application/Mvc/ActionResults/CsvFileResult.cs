using KhumaloCraft.Application.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace KhumaloCraft.Application.Mvc.ActionResults;

public static class CsvFileResult
{
    /// <summary>
    /// Creates a new CsvFileResult<<typeparamref name="T"/>> which will write unbuffered to the response stream by default.
    /// </summary>
    /// <typeparam name="T">The row type.</typeparam>
    /// <param name="items">Enumerable of type <typeparamref name="T"/>.</param>
    /// <param name="filename">The CSV filename, with or without the extension.</param>
    /// <param name="classMap">An optional classmap instance to map output columns.</param>
    /// <returns>A new CsvFileResult<<typeparamref name="T"/>>, which inherits FileResult.</returns>
    public static CsvFileResult<T> Create<T>(IEnumerable<T> items, string filename, CsvHelper.Configuration.ClassMap<T> classMap = null)
    {
        return new CsvFileResult<T>(items, filename, classMap);
    }

    /// <summary>
    /// Creates a new CsvFileResult<<typeparamref name="T"/>, <typeparamref name="TClassMap"/>> which will write unbuffered to the response stream by default.
    /// </summary>
    /// <typeparam name="T">The row type.</typeparam>
    /// <typeparam name="TClassMap">An class which implements ClassMap<<typeparamref name="T"/>>.</typeparam>
    /// <param name="items">Enumerable of type <typeparamref name="T"/>.</param>
    /// <param name="filename">The CSV filename, with or without the extension.</param>
    /// <returns>A new CsvFileResult<<typeparamref name="T"/>, <typeparamref name="TClassMap"/>>, which inherits CsvFileResult<<typeparamref name="T"/>>.</returns>
    public static CsvFileResult<T, TClassMap> Create<T, TClassMap>(IEnumerable<T> items, string filename)
        where TClassMap : CsvHelper.Configuration.ClassMap<T>
    {
        return new CsvFileResult<T, TClassMap>(items, filename);
    }
}

public class CsvFileResult<T> : FileResult
{
    private IEnumerable<T> _rows;
    private readonly CsvHelper.Configuration.ClassMap<T> _classMap;

    public CsvFileResult(IEnumerable<T> rows, string filename, CsvHelper.Configuration.ClassMap<T> classMap = null)
        : base(Constants.MimeTypes.Csv.Default)
    {
        _rows = rows;
        FileDownloadName = filename.EndsWith(".csv") ? filename : $"{filename}.csv";
        _classMap = classMap;
    }

    private void SetContentHeaders(HttpResponse response)
    {
        var typedHeaders = response.GetTypedHeaders();
        typedHeaders.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = FileDownloadName
        };
        typedHeaders.ContentType = new MediaTypeHeaderValue(ContentType);
    }

    protected virtual void CsvWriterConfiguration(CsvHelper.Configuration.IWriterConfiguration configuration)
    {
        if (_classMap is not null)
        {
            configuration.RegisterClassMap(_classMap);
        }
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;

        SetContentHeaders(response);

        await using (var streamWriter = new StreamWriter(response.Body, System.Text.Encoding.Default, 1024, true))
        using (var writer = new CsvWriter(streamWriter, CsvWriterConfiguration, true))
        {
            await writer.WriteAsync(_rows);
        }
    }
}

public class CsvFileResult<T, TClassMap> : CsvFileResult<T>
    where TClassMap : CsvHelper.Configuration.ClassMap<T>
{
    public CsvFileResult(IEnumerable<T> rows, string filename)
        : base(rows, filename)
    { }

    protected override void CsvWriterConfiguration(CsvHelper.Configuration.IWriterConfiguration configuration)
    {
        configuration.RegisterClassMap<TClassMap>();
    }
}