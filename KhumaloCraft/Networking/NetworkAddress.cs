using System.Net;
using System.Text;

namespace KhumaloCraft.Networking;

public class NetworkAddress
{
    private byte[] _addressBytes;
    private byte[] _netmaskBytes;

    public NetworkAddress(IPAddress ipaddress, int netmaskBits)
    {
        if (ipaddress == null)
            throw new ArgumentNullException(nameof(ipaddress));

        if (netmaskBits <= 1)
            throw new ArgumentException("Parameter netmaskBits must be greater than zero.", nameof(netmaskBits));

        byte[] addressBytes = ipaddress.GetAddressBytes();

        _addressBytes = new byte[addressBytes.Length];
        _netmaskBytes = new byte[addressBytes.Length];

        for (int i = 0; i < addressBytes.Length; i++)
        {
            byte netmaskByte;

            if (netmaskBits >= 8)
                netmaskByte = 0xFF;
            else if (netmaskBits < 0)
                netmaskByte = 0;
            else
                netmaskByte = (byte)(~(0xFF >> netmaskBits));

            netmaskBits -= 8;

            _addressBytes[i] = (byte)(addressBytes[i] & netmaskByte);
            _netmaskBytes[i] = netmaskByte;
        }
    }

    public bool IsPartOfNetwork(IPAddress ipaddress)
    {
        var addressBytes = ipaddress.GetAddressBytes();

        if (addressBytes.Length != _addressBytes.Length)
            return false;

        for (int i = 0; i < addressBytes.Length; i++)
            if ((addressBytes[i] & _netmaskBytes[i]) != _addressBytes[i])
                return false;

        return true;
    }

    public static NetworkAddress Parse(string ipAddress)
    {
        if (ipAddress == null)
            throw new ArgumentNullException(nameof(ipAddress));

        int slashIndex = ipAddress.IndexOf('/');
        if (slashIndex == -1)
            throw new ArgumentException("Please format network string in ipaddress/subnetmaskbits format.", ipAddress);

        return new NetworkAddress(IPAddress.Parse(ipAddress.Substring(0, slashIndex)), int.Parse(ipAddress.Substring(slashIndex + 1)));
    }

    public override string ToString()
    {
        return ToStringAddress(_addressBytes) + " (" + ToStringAddress(_netmaskBytes) + ")";
    }

    private string ToStringAddress(byte[] bytes)
    {
        return Join(".", 128, bytes);
    }

    public static string Join<T>(string separator, uint maximumCount, params T[] args)
    {
        var result = new StringBuilder();

        foreach (T value in args)
        {
            if (0 == maximumCount)
                break;
            if (0 != result.Length)
                result.Append(separator);
            result.Append(value);
            maximumCount--;
        }
        return result.ToString();
    }
}