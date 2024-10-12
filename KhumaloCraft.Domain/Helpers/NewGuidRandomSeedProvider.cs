namespace KhumaloCraft.Domain;

public class NewGuidRandomSeedProvider : IRandomSeedProvider
{
    public string Seed => Guid.NewGuid().ToString();
}