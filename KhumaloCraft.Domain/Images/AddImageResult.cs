using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

public class AddImageResult
{
    public AddImageResult(int referenceId, uint width, uint height)
    {
        ReferenceId = referenceId;
        Width = width;
        Height = height;
    }

    public AddImageResult(int referenceId, ImageSize imageSize)
        : this(referenceId, imageSize.Width, imageSize.Height)
    {
    }

    public int ReferenceId { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }
}