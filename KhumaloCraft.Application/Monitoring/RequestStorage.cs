using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Monitoring;

public class RequestStorage : IRequestStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public T Get<T>() where T : class, new()
    {
        T value;

        if (_httpContextAccessor.HttpContext != null)
        {
            value = _httpContextAccessor.HttpContext.Items[typeof(T)] as T;

            if (value == null)
            {
                value = new T();

                _httpContextAccessor.HttpContext.Items[typeof(T)] = value;
            }
        }
        else
        {
            value = new T();
        }

        return value;
    }
}