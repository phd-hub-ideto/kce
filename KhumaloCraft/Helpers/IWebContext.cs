using System.Net;

namespace KhumaloCraft;

public interface IWebContext
{
    IPAddress UserHostIPAddress();
    bool IsUnitTest { get; }
    bool DisableObjectCache { get; set; }
}