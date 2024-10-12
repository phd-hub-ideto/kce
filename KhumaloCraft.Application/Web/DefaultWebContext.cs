using KhumaloCraft.Dependencies;
using System.Net;

namespace KhumaloCraft.Application.Web;

[Singleton(Contract = typeof(IWebContext))]
public class DefaultWebContext(IPAddressHelper ipAddressHelper) : IWebContext
{
    public IPAddress UserHostIPAddress()
    {
        return ipAddressHelper.GetUserIp();
    }

    public bool IsUnitTest
    {
        get
        {
            return false;
        }
    }

    public bool DisableObjectCache
    {
        get
        {
            return false;
        }
        set
        {
            throw new Exception($"Cannot set this property {nameof(DisableObjectCache)}, in this context.");
        }
    }
}