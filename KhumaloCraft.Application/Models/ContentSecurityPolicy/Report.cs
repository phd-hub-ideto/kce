using System.Text.Json.Serialization;

namespace KhumaloCraft.Application.Models.ContentSecurityPolicy;

public class Report
{
    [JsonPropertyName("document-uri")]
    public string DocumentUri { get; set; }

    [JsonPropertyName("referrer")]
    public string Referrer { get; set; }

    [JsonPropertyName("violated-directive")]
    public string ViolatedDirective { get; set; }

    [JsonPropertyName("effective-directive")]
    public string EffectiveDirective { get; set; }

    [JsonPropertyName("original-policy")]
    public string OriginalPolicy { get; set; }

    [JsonPropertyName("blocked-uri")]
    public string BlockedUri { get; set; }

    [JsonPropertyName("status-code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("column-number")]
    public int ColumnNumber { get; set; }

    [JsonPropertyName("disposition")]
    public string Disposition { get; set; }

    [JsonPropertyName("line-number")]
    public int LineNumber { get; set; }

    [JsonPropertyName("script-sample")]
    public string ScriptSample { get; set; }

    [JsonPropertyName("source-file")]
    public string SourceFile { get; set; }
}