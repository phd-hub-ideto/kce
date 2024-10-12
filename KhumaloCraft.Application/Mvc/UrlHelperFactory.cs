using KhumaloCraft.Dependencies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace KhumaloCraft.Application.Mvc;

[Singleton(Contract = typeof(IUrlHelperFactory<ActionContext>))]
internal class UrlHelperFactory : IUrlHelperFactory<ActionContext>
{
    IUrlHelper IUrlHelperFactory<ActionContext>.Create(IHtmlHelper htmlHelper)
    {
        return new UrlHelper(htmlHelper.ViewContext);
    }

    IUrlHelper IUrlHelperFactory<ActionContext>.Create(ActionContext context)
    {
        return new UrlHelper(context);
    }
}