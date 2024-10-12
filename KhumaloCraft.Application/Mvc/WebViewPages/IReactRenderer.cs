namespace KhumaloCraft.Application.Mvc.WebViewPages;

using Microsoft.AspNetCore.Html;

public interface IReactRenderer
{
    bool ShouldAllowServerSideRender();

    IHtmlContent Render<T>(string componentName, T props, bool clientOnly = false, string containerId = null) where T : class;
}