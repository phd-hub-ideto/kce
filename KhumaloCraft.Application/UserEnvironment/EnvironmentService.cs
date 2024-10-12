using KhumaloCraft.Application.Platforms;
using KhumaloCraft.Application.Session;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.UserEnvironment;

namespace KhumaloCraft.Application.UserEnvironment;

[Singleton(Contract = typeof(IEnvironmentService))]
public class EnvironmentService(
    IPlatformDeterminator platformDeterminator,
    IPrincipalResolver principalResolver,
    IWebContext webContext,
    IUniqueUserTracker uniqueUserTracker) : IEnvironmentService
{
    public EnvironmentInformation GetEnvironment()
    {
        var platform = platformDeterminator.Determine();

        return new EnvironmentInformation
        {
            UserId = principalResolver.GetUserId(),
            IPAddress = webContext.UserHostIPAddress(),
            Platform = platform,
            UniqueUserId = uniqueUserTracker.GetUniqueUserId()
        };
    }
}