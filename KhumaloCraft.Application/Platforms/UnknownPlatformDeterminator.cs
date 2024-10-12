using KhumaloCraft.Domain.Platforms;

namespace KhumaloCraft.Application.Platforms;

public class UnknownPlatformDeterminator : IPlatformDeterminator
{
    public Platform Determine() => Platform.Unknown;
}