using KhumaloCraft.Application.Authentication;
using KhumaloCraft.Application.Http;
using KhumaloCraft.Application.Portal.Models.Error;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace KhumaloCraft.Application.Portal.Controllers;

public partial class BaseController : Controller
{
    private readonly UrlMarketingQueryStringPreserver _urlMarketingQueryStringPreserver;

    public BaseController()
    {
        _urlMarketingQueryStringPreserver = new UrlMarketingQueryStringPreserver();
    }

    public override RedirectResult Redirect(string url)
    {
        var uri = new Uri(Request.GetEncodedUrl());
        return base.Redirect(_urlMarketingQueryStringPreserver.Preserve(uri, url));
    }

    public override RedirectResult RedirectPermanent(string url)
    {
        var uri = new Uri(Request.GetEncodedUrl());
        return base.RedirectPermanent(_urlMarketingQueryStringPreserver.Preserve(uri, url));
    }

    public virtual ActionResult CreateErrorPage(HttpStatusCode httpStatusCode, string errorReference = null)
    {
        HttpContext.Response.StatusCode = (int)httpStatusCode;

        return View("../Error/ErrorPage", ErrorModel.Create(httpStatusCode, errorReference));
    }

    protected Dictionary<string, List<string>> GetValidationErrors(ModelStateDictionary modelState)
    {
        var dictionary = new Dictionary<string, List<string>>();

        foreach (var key in modelState.Keys)
        {
            if (modelState.TryGetValue(key, out var value))
            {
                var propertyName = key;

                if (value.Errors.Count() > 0)
                {
                    var errorMessages = new List<string>();

                    for (var i = 0; i < value.Errors.Count; i++)
                    {
                        errorMessages.Add(value.Errors[i].ErrorMessage);
                    }
                    dictionary.Add(propertyName, errorMessages);
                }
            }
        }
        return dictionary;
    }

    protected ActionResult Logout(
        IFormsAuthenticator formsAuthenticator,
        IHttpRequestProvider httpRequestProvider)
    {
        formsAuthenticator.Signout();

        if (httpRequestProvider.UrlReferrer != null)
        {
            return Redirect(httpRequestProvider.UrlReferrer.PathAndQuery);
        }

        return Redirect(Constants.Pages.BaseHome);
    }

    protected bool HasAccess(AdministratorPermission permission)
    {
        return PermissionManager.Current.HasPermission(permission);
    }

    protected bool HasAccess(UserPermission permission)
    {
        return PermissionManager.Current.HasPermission(permission);
    }
}