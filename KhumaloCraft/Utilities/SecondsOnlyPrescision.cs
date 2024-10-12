namespace KhumaloCraft.Utilities;

public static class SecondsOnlyPrescision
{
    public static int Compare(DateTime left, DateTime right)
    {
        return left.AddTicks(-(left.Ticks % TimeSpan.TicksPerSecond)).CompareTo(right.AddTicks(-(right.Ticks % TimeSpan.TicksPerSecond)));
    }

    public static bool Equals(DateTime left, DateTime right)
    {
        return Compare(left, right) == 0;
    }
}
