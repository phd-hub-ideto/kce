using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace KhumaloCraft.Application.Mvc.WebViewPages;

public abstract class RazorViewPage<T> : RazorPage<T>
{
    private static readonly System.Text.Json.JsonSerializerOptions _options = new System.Text.Json.JsonSerializerOptions()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    public IHtmlContent ToJson(object value)
    {
        return new HtmlString(System.Text.Json.JsonSerializer.Serialize(value, _options));
    }
}