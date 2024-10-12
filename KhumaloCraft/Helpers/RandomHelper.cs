namespace KhumaloCraft;

public static class RandomHelper
{
    private static readonly Random _globalRandom = new Random();

    [ThreadStatic]
    private static Random _localRandom;

    private static Random GetRandom()
    {
        var random = _localRandom;

        if (random == null)
        {
            int seed;

            lock (_globalRandom)
            {
                seed = _globalRandom.Next();
            }

            _localRandom = random = new Random(seed);
        }

        return random;
    }

    public static int Next()
    {
        return GetRandom().Next();
    }

    public static int Next(int maxValue)
    {
        return GetRandom().Next(maxValue);
    }

    public static int Next(int minValue, int maxValue)
    {
        return GetRandom().Next(minValue, maxValue);
    }
}
