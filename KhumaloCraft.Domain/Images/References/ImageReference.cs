namespace KhumaloCraft.Domain.Images;

[Serializable]
public class ImageReference
{
    public ImageId ImageId { get; }
    public int ReferenceId { get; }

    public ImageReference(ImageId imageId, int referenceId)
    {
        ImageId = imageId;
        ReferenceId = referenceId;
    }

    public static ImageReference Create(int? imageId, int? referenceId)
    {
        if (imageId == null ^ referenceId == null) throw new InvalidOperationException($"{nameof(imageId)} and {nameof(referenceId)} most both be specified or both be null.");

        if (imageId.HasValue && referenceId.HasValue)
        {
            return new ImageReference(new ImageId(imageId.Value), referenceId.Value);
        }

        return null;
    }

    public static ImageReference[] Create(int[] imageIds, int[] referenceId)
    {
        if (imageIds.Length != referenceId.Length) throw new InvalidOperationException($"Arrays {nameof(imageIds)} and {nameof(referenceId)} must have the same length.");

        var imageReferences = new ImageReference[imageIds.Length];
        for (int i = 0; i < imageIds.Length; i++)
        {
            imageReferences[i] = new ImageReference(new ImageId(imageIds[i]), referenceId[i]);
        }

        return imageReferences;
    }
}
