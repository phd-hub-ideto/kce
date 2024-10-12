using KhumaloCraft.Domain.Platforms;
using System.Net;

namespace KhumaloCraft.Domain.UserEnvironment;

public class EnvironmentInformation
{
    public int? UserId { get; set; }
    public IPAddress IPAddress { get; set; }
    public Platform Platform { get; set; }
    public int UIContextId => (int)Platform;
    public string UniqueUserId { get; set; }
}