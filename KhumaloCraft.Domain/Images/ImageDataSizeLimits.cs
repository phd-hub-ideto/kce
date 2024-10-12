namespace KhumaloCraft.Domain.Images;

public static class ImageDataSizeLimits
{
    public static int MinimumImageSizeB => Settings.Instance.MinimumImageSizeB;

    public static uint MaximumImageSizeMB => Settings.Instance.MaximumImageSizeMB;

    public static uint MaximumImageSize => MaximumImageSizeMB * 1024U * 1024U;

    public static uint MaximumImagePayloadSizeMB => Settings.Instance.MaximumImagePayloadSizeMB;

    public static ulong MaximumImagePayloadSize => MaximumImagePayloadSizeMB * 1024UL * 1024UL;
}