using KhumaloCraft.Dependencies;
using KhumaloCraft.Http;
using System.Net;

namespace KhumaloCraft;

[Singleton]
public class IPAddressHelper(IHttpRequestProvider httpRequestProvider)
{
    public IPAddress GetUserIp()
    {
        var xForwardedForHeader = httpRequestProvider.XForwardedFor;

        if (!string.IsNullOrWhiteSpace(xForwardedForHeader))
        {
            var addresses = xForwardedForHeader.Split(',').Reverse().ToList();

            for (int i = 0; i < addresses.Count; i++)
            {
                bool isLast = i + 1 == addresses.Count;
                var address = addresses[i];

                // TODO-L: ipv6?
                var ipString = address.Contains(':') ? address.Substring(0, address.IndexOf(":")) : address;

                if (IPAddress.TryParse(ipString.Trim(), out var ipAddress))
                {
                    if (isLast)
                    {
                        return ipAddress;
                    }
                }
            }
        }

        return httpRequestProvider.UserHostAddress;
    }
}