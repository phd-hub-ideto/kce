using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KhumaloCraft.Application.Mvc;

public interface IUrlHelperFactory<TContext>
{
    IUrlHelper Create(IHtmlHelper htmlHelper);
    IUrlHelper Create(TContext context);
}