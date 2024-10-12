namespace KhumaloCraft.Application.ImageServer.Helpers;

public static class GeneralHelpers
{
    public static T Minimum<T>(T t1, T t2) where T : IComparable
    {
        if (t1.CompareTo(t2) < 0)
            return t1;

        return t2;
    }
}