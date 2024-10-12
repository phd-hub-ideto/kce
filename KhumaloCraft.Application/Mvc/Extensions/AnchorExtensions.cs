using KhumaloCraft.Application.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Text.Encodings.Web;

namespace KhumaloCraft.Application.Mvc.Extensions;

public static class AnchorExtensions
{
    public static IHtmlContent Anchor<TModel>(this IHtmlHelper<TModel> htmlHelper, HtmlAnchorData anchorData, object htmlAttributes = null)
    {
        return htmlHelper.Anchor(anchorData, (model) => new HelperResult((writer) => writer.WriteAsync(anchorData.Text)), htmlAttributes);
    }

    public static IHtmlContent Anchor<TModel>(this IHtmlHelper<TModel> htmlHelper, HtmlAnchorData anchorData, string linkText, object htmlAttributes = null)
    {
        return htmlHelper.Anchor(anchorData, (model) => new HelperResult((writer) => writer.WriteAsync(linkText)), htmlAttributes);
    }

    public static IHtmlContent Anchor<TModel>(this IHtmlHelper<TModel> htmlHelper, HtmlAnchorData anchorData, Func<TModel, HelperResult> linkText, object htmlAttributes = null)
    {
        TagBuilder anchor = new TagBuilder("a");
        anchor.Attributes["href"] = anchorData.Url;
        anchor.Attributes["title"] = anchorData.Title;
        anchor.InnerHtml.AppendHtml(linkText(htmlHelper.ViewData.Model));
        if (anchorData.Target != null)
        {
            anchor.Attributes["target"] = anchorData.Target;
        }

        if (htmlAttributes != null)
        {
            foreach (var htmlAttribute in new RouteValueDictionary(htmlAttributes))
            {
                if (htmlAttribute.Value != null)
                {
                    anchor.Attributes[htmlAttribute.Key] = htmlAttribute.Value.ToString();
                }
            }
        }
        return new TagContent(anchor);
    }

    private class TagContent : IHtmlContent
    {
        private readonly TagBuilder _anchor;

        public TagContent(TagBuilder anchor)
        {
            _anchor = anchor;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            _anchor.WriteTo(writer, encoder);
        }
    }
}