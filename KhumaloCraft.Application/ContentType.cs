namespace KhumaloCraft;

public static class ContentType
{
    public const string Css = "text/css";
    public const string Gif = "image/gif";
    public const string Png = "image/png";
    public const string Jpeg = "image/jpeg";
    public const string Icon = "image/ico";
    public const string Webp = "image/webp";
    [Obsolete("Use 'application/javascript' in its place.", DiagnosticId = "ATTJS")]
    public const string Javascript = "text/javascript";
    public const string Text = "text/plain";
    public const string Pdf = "application/pdf";
    public const string Json = "application/json";
    public const string Xml = "text/xml";
    public const string Svg = "image/svg+xml";
    public const string Html = "text/html";
    [Obsolete("Modern browsers ignore this value.", DiagnosticId = "ATEJS")]
    public const string ExperimentalJavascript = "text/x-javascript";
    public const string CspReport = "application/csp-report";
    public const string ApplicationJavascript = "application/javascript";
    public const string Atom = "application/atom+xml";
    public const string Rss = "application/rss+xml";
}