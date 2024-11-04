using KhumaloCraft.Dependencies;
using System.Transactions;

namespace KhumaloCraft.Domain.Images;

[Singleton]
internal class ImageReferenceManager : IImageReferenceManager
{
    private readonly IImageRepository _imageRepository;

    public ImageReferenceManager(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public int? PersistReference(ImageHandle imageHandle)
    {
        var accumulator = new ImageReferenceAccumulator();

        UpdateReference(imageHandle, accumulator);
        CommitAll(accumulator);

        return imageHandle.ReferenceId;
    }

    public void UpdateReferenceCollection(IEnumerable<int> oldImageReferences, IEnumerable<int> newImageReferences)
    {
        var accumulator = new ImageReferenceAccumulator();

        foreach (var oldImage in oldImageReferences)
        {
            accumulator.Remove(oldImage);
        }

        foreach (var newImage in newImageReferences)
        {
            accumulator.Add(newImage);
        }

        CommitAll(accumulator);
    }

    private void CommitAll(ImageReferenceAccumulator accumulator)
    {
        using var scope = new TransactionScope();

        if (accumulator.Removed.Any())
        {
            _imageRepository.Unreference(accumulator.Removed);
        }

        if (accumulator.Added.Any())
        {
            _imageRepository.Reference(accumulator.Added);
        }

        scope.Complete();
    }

    private static void UpdateReference(ImageHandle imageHandle, ImageReferenceAccumulator accumulator)
    {
        IUpdatableImageHandle updatableImageHandle = imageHandle;
        updatableImageHandle.Commit(accumulator);
    }

    public int Reference(int imageReferenceId)
    {
        _imageRepository.Reference(new[] { imageReferenceId });
        return imageReferenceId;
    }

    public void Unreference(IEnumerable<int> imageReferenceIds)
    {
        _imageRepository.Unreference(imageReferenceIds);
    }

    public void Unreference(ImageHandle imageHandle)
    {
        var accumulator = new ImageReferenceAccumulator();
        UpdateReference(imageHandle, accumulator);
        if (imageHandle.ReferenceId != null)
        {
            accumulator.Remove(imageHandle.ReferenceId.Value);
        }
        CommitAll(accumulator);
    }
}
