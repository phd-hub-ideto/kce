using KhumaloCraft.Domain;
using KhumaloCraft.Http;

namespace KhumaloCraft.Application.Http;

public class HttpContextRandomSeedProvider(
    IHttpContextProvider httpContextProvider
    ) : IRandomSeedProvider
{
    public string Seed => httpContextProvider.GetSessionId();
}