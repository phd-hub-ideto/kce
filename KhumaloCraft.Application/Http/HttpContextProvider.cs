using System.Collections.Specialized;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;

namespace KhumaloCraft.Application.Http;

public class HttpContextProvider : IHttpContextProvider
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;

    public HttpContextProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
    {
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
    }

    public bool CanGetHttpContext
    {
        get
        {
            return _httpContextAccessor?.HttpContext != null;
        }
    }

    private static readonly object _eventDataRequestId = new object();
    public string RequestId
    {
        get
        {
            if (CanGetHttpContext)
            {
                //Assign a request id to we can correlate multiple events together.

                var requestId = GetItem(_eventDataRequestId) as string;
                if (requestId is null)
                {
                    requestId = Guid.NewGuid().ToString();
                    SetItem(_eventDataRequestId, requestId);
                    return requestId;
                }
                else
                {
                    return requestId;
                }
            }
            return null;
        }
    }

    public bool AllowCookiesInResponse
    {
        get
        {
            if (_httpContextAccessor.HttpContext.Items[nameof(AllowCookiesInResponse)] == null)
                return false;

            return ((bool)_httpContextAccessor.HttpContext.Items[nameof(AllowCookiesInResponse)]) == true;
        }
        set
        {
            if (value)
                _httpContextAccessor.HttpContext.Items[nameof(AllowCookiesInResponse)] = value;
            else
                _httpContextAccessor.HttpContext.Items[nameof(AllowCookiesInResponse)] = null;
        }
    }

    public NameValueCollection Headers()
    {
        var result = new NameValueCollection();

        foreach (var kvp in _httpContextAccessor.HttpContext.Request.Headers)
        {
            result.Add(kvp.Key.ToString(), kvp.Value.ToString());
        }

        return result;
    }

    public object GetItem(object key) => _httpContextAccessor?.HttpContext?.Items[key];

    public void SetItem(object key, object value)
    {
        if (CanGetHttpContext)
        {
            _httpContextAccessor.HttpContext.Items[key] = value;
        }
    }

    public string GetHttpHeaders(string name) => _httpContextAccessor?.HttpContext?.Request.Headers[name];

    public string MapPath(string virtualPath)
    {
        if (CanGetHttpContext)
        {
            return Path.Combine(_hostEnvironment.WebRootPath, virtualPath);
        }
        return null;
    }

    public string GetCurrentCookie(string name)
    {
        if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(name))
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[name];
        }

        return null;
    }

    public void SetSessionValue(string key, object value)
    {
        if (value is not null)
        {
            var jsonData = JsonSerializer.Serialize(value);

            var data = System.Text.Encoding.UTF8.GetBytes(jsonData);

            _httpContextAccessor.HttpContext.Session.Set(key, data);
        }
        else
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }
    }

    public object GetSessionValue(string key)
    {
        if (_httpContextAccessor.HttpContext.Session.TryGetValue(key, out var data) && data is not null)
        {
            return JsonSerializer.Deserialize<object>(data);
        }

        return null;
    }

    public void ClearSessionValue(string key)
    {
        _httpContextAccessor.HttpContext.Session.Remove(key);
    }

    public bool SessionExists => CanGetHttpContext && _httpContextAccessor.HttpContext.Features.Get<ISessionFeature>()?.Session != null;

    public void ClearSession()
    {
        _httpContextAccessor.HttpContext.Session = null;
    }

    public string RequestPathAndQuery
    {
        get
        {
            if (CanGetHttpContext)
            {
                return _httpContextAccessor.HttpContext.Request.GetEncodedPathAndQuery();
            }
            return string.Empty;
        }
    }

    public bool HandlerAvailable
    {
        get
        {
            if (CanGetHttpContext)
            {
                return _httpContextAccessor.HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseFeature>().HasStarted;
            }
            return false;
        }
    }

    public string Username
    {
        get
        {
            if (CanGetHttpContext && _httpContextAccessor.HttpContext.User is not null)
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
            return string.Empty;
        }
    }

    NameValueCollection IHttpContextProvider.Headers => Headers();

    public string GetSessionId()
    {
        if (SessionExists)
        {
            return _httpContextAccessor.HttpContext.Session.Id;
        }
        return null;
    }
}
