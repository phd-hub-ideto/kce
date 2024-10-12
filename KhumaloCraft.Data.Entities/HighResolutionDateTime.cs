using System.Runtime.InteropServices;

namespace KhumaloCraft.Data.Entities;

internal static class HighResolutionDateTime
{
    public static bool IsAvailable { get; }

    static HighResolutionDateTime()
    {
        try
        {
            GetSystemTimePreciseAsFileTime(out var _);
            IsAvailable = true;
        }
        catch (EntryPointNotFoundException)
        {
            // Not running Windows 8 or higher.
            IsAvailable = false;
        }
    }

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    public static DateTime Now
    {
        get
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException("High-resolution clock isn't available.");
            }

            GetSystemTimePreciseAsFileTime(out var filetime);

            // TODO-L: should probably use current culture's UTC offset
            return DateTime.FromFileTimeUtc(filetime).AddHours(2);
        }
    }
}