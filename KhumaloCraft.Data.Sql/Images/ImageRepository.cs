using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;
using System.Transactions;

namespace KhumaloCraft.Data.Sql.Images;

public class ImageRepository : IImageRepository
{
    private readonly IImageBlobRepository _imageBlobRepository;

    public ImageRepository(IImageBlobRepository imageBlobRepository)
    {
        _imageBlobRepository = imageBlobRepository;
    }

    public bool TryGetImageByImageId(ImageId imageId, out ImageData imageData)
    {
        return _imageBlobRepository.TryGetImageByImageId(imageId, out imageData);
    }

    public async Task<(bool result, ImageData imageData)> TryGetImageByImageIdAsync(ImageId imageId)
    {
        return await _imageBlobRepository.TryGetImageByImageIdAsync(imageId).ConfigureAwait(false);
    }

    public ImageReference AddImage(ImageData imageData)
    {
        if (imageData == null) throw new ArgumentNullException(nameof(imageData));

        var imageId = _imageBlobRepository.AddImage(imageData);

        return AddImageReferenceIdToImageId(imageId);
    }

    public ImageReference[] AddImages(IEnumerable<ImageData> datas)
    {
        if (datas is null) throw new ArgumentNullException(nameof(datas));

        using var scope = new TransactionScope();

        var referenceIds = new List<ImageReference>();

        foreach (var imageData in datas)
        {
            referenceIds.Add(AddImage(imageData));
        };

        scope.Complete();

        return referenceIds.ToArray();
    }

    public void Unreference(IEnumerable<int> referenceIds)
    {
        if (referenceIds is null) throw new ArgumentNullException(nameof(referenceIds));

        using var scope = DalScope.Begin();

        var serverDate = scope.ServerDate();

        var dalImages = scope.KhumaloCraft.ImageReference
                             .Where(i =>
                                 referenceIds.Contains(i.ReferenceId) &&
                                 i.DeletedOn == null);

        foreach (var dalImage in dalImages)
        {
            dalImage.DeletedOn = serverDate;
        }

        scope.KhumaloCraft.ImageReference.UpdateRange(dalImages);

        scope.Commit();
    }

    public void Reference(IEnumerable<int> referenceIds)
    {
        if (referenceIds is null) throw new ArgumentNullException(nameof(referenceIds));

        using var scope = DalScope.Begin();

        var serverDate = scope.ServerDate();

        var dalImages = scope.KhumaloCraft.ImageReference
                             .Where(i =>
                                 referenceIds.Contains(i.ReferenceId) &&
                                 i.DeletedOn == null);

        foreach (var dalImage in dalImages)
        {
            dalImage.DeletedOn = null;
        }

        scope.KhumaloCraft.ImageReference.UpdateRange(dalImages);

        scope.Commit();
    }

    public bool TryGetImageIdByReferenceId(int referenceId, out ImageId imageId)
    {
        DalImageReference dalImage;

        using (var scope = DalScope.Begin())
        {
            dalImage = scope.KhumaloCraft.ImageReference
                            .SingleOrDefault(i => i.ReferenceId == referenceId);

            if (dalImage != null && dalImage.ImageId != null)
            {
                imageId = new ImageId(dalImage.ImageId.Value);

                return true;
            }
        }

        imageId = default;

        return false;
    }

    public ImageReference AddImageReferenceIdToImageId(ImageId imageId)
    {
        using var scope = DalScope.Begin();

        var dalImageReference = new DalImageReference
        {
            ImageId = imageId.Value,
            DeletedOn = scope.ServerDate()
        };

        scope.KhumaloCraft.ImageReference.Add(dalImageReference);

        scope.Commit();

        return new ImageReference(imageId, dalImageReference.ReferenceId);
    }

    public List<ImageReference> FetchDeletedReferences(int count, DateTime deletedBefore)
    {
        using var dalScope = DalScope.Begin(TimeSpan.FromMinutes(5).Seconds);

        return [.. dalScope.KhumaloCraft.ImageReference
                    .Where(i =>
                        i.DeletedOn < deletedBefore &&
                        i.ImageId != null)
                    .Take(count)
                    .Select(item => new ImageReference(new ImageId(item.ImageId.Value), item.ReferenceId))];
    }

    public int Delete(IEnumerable<ImageReference> referenceIds)
    {
        using var dalScope = DalScope.Begin();

        var deletedImageReferenceIds = referenceIds.Select(item => item.ReferenceId);

        var entitesToRemove = dalScope.KhumaloCraft.ImageReference.Where(i => deletedImageReferenceIds.Contains(i.ReferenceId));

        dalScope.KhumaloCraft.RemoveRange(entitesToRemove);

        var referencedImageIds = dalScope.KhumaloCraft.ImageReference
                                    .Where(i =>
                                        i.ImageId != null &&
                                        referenceIds.Select(item => item.ImageId.Value).Contains(i.ImageId.Value))
                                    .Select(i => new ImageId(i.ImageId.Value));

        var unreferencedImageIds = referenceIds.Select(item => item.ImageId)
            .Except(referencedImageIds)
            .ToList();

        _imageBlobRepository.DeleteImages(unreferencedImageIds);

        dalScope.Commit();

        return unreferencedImageIds.Count;
    }

    public bool IsReferenced(int referenceId)
    {
        using var dalScope = DalScope.Begin();

        var dalImageReference = dalScope.KhumaloCraft.ImageReference.Single(i => i.ReferenceId == referenceId);

        return dalImageReference.DeletedOn == null;
    }

    public ImageId FetchOneImageId()
    {
        using var dalScope = DalScope.Begin();

        return new ImageId(dalScope.KhumaloCraft.Image.First().Id);
    }

    public (int Width, int Height) GetImageDimensionsById(ImageId imageId)
    {
        using var scope = DalScope.Begin();

        var dalImage = scope.KhumaloCraft.Image.Single(i => i.Id == imageId.Value);

        return (dalImage.Width, dalImage.Height);
    }
}