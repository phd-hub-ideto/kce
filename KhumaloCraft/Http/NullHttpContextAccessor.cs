using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Http;

public class NullHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext HttpContext { get => null; set => throw new NotImplementedException(); }
}