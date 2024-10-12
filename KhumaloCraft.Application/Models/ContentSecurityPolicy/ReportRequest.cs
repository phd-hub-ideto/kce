using System.Text.Json.Serialization;

namespace KhumaloCraft.Application.Models.ContentSecurityPolicy;

public class ReportRequest
{
    [JsonPropertyName("csp-report")]
    public Report CspReport { get; set; }
}