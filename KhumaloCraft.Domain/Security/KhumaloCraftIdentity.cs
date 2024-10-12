using System.Security.Principal;

namespace KhumaloCraft.Domain.Security;

[Serializable]
public class KhumaloCraftIdentity : IIdentity
{
    public KhumaloCraftIdentity(string username)
    {
        Name = username;
    }

    public string AuthenticationType
    {
        // Custom Authentication
        get { return "KhumaloCraft"; }
    }

    public bool IsAuthenticated
    {
        // The presence of an Identity implies that authentication has been passed.
        get { return true; }
    }

    public string Name { get; private set; }
}