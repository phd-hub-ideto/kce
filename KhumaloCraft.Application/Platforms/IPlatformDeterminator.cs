using KhumaloCraft.Domain.Platforms;

namespace KhumaloCraft.Application.Platforms;

public interface IPlatformDeterminator
{
    Platform Determine();
}