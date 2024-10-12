using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;
using System.Security.Cryptography;

namespace KhumaloCraft.Data.Sql.Images;

public class ImageBlobRepository : IImageBlobRepository
{
    private readonly FallbackImageService _fallbackImageService;
    private readonly IImageDataRepository _imageDataRepository;

    public ImageBlobRepository(FallbackImageService fallbackImageService, IImageDataRepository imageDataRepository)
    {
        _fallbackImageService = fallbackImageService;
        _imageDataRepository = imageDataRepository;
    }

    public bool TryGetImageByImageId(ImageId imageId, out ImageData imageData)
    {
        using var scope = DalScope.Begin();

        var dalImage = scope.KhumaloCraft.Image
                        .Where(i => i.Id == imageId.Value)
                        .Select(item => new { item.ImageTypeId, item.Date })
                        .SingleOrDefault();

        if (_imageDataRepository.TryGet(imageId, out imageData))
        {
            if (dalImage is null)
            {
                throw new InvalidOperationException($"Image blob {imageId.Value} exits in storage account but has no metadata in Image database.");
            }

            return true;
        }
        else if (_fallbackImageService.FallbackEnabled)
        {
            if (_fallbackImageService.TryGetFallbackImage(imageId, out var fallbackImage))
            {
                imageData = fallbackImage;
            }
            else
            {
                // This image is only ever needed for development when production no longer has the image.
                imageData = new ImageData(FallbackNoImageResources.Blank, contentType: ImageContentType.Png);
            }
            return true;
        }
        else if (dalImage is not null)
        {
            throw new InvalidOperationException($"Image {imageId.Value} exists in database, but is missing in the storage account.");
        }

        imageData = null;

        return false;
    }

    public async Task<(bool result, ImageData imageData)> TryGetImageByImageIdAsync(ImageId imageId)
    {
        using var scope = DalScope.Begin();

        var dalImage = scope.KhumaloCraft.Image
                            .Where(i => i.Id == imageId.Value)
                            .Select(i => new { i.ImageTypeId, i.Date })
                            .FirstOrDefault();

        var (result, imageData) = await _imageDataRepository.TryGetAsync(imageId).ConfigureAwait(false);

        if (result)
        {
            if (dalImage is null)
            {
                throw new InvalidOperationException($"Image blob {imageId.Value} exits in storage account but has no metadata in Image database.");
            }

            return (true, imageData);
        }
        else if (_fallbackImageService.FallbackEnabled)
        {
            if (_fallbackImageService.TryGetFallbackImage(imageId, out var fallbackImage))
            {
                imageData = fallbackImage;
            }
            else
            {
                // This image is only ever needed for development when production no longer has the image.
                imageData = new ImageData(FallbackNoImageResources.Blank, contentType: ImageContentType.Png);
            }
            return (true, imageData);
        }
        else if (dalImage is not null)
        {
            throw new InvalidOperationException($"Image {imageId.Value} exists in database, but is missing in the storage account.");
        }

        return (false, null);
    }

    public ImageId AddImage(ImageData imageData)
    {
        ImageId imageId;

        using (var scope = DalScope.Begin())
        {
            var dataLength = imageData.Data.Length;

            var hash = ComputeHash(imageData);

            if (TryFetch(scope, dataLength, hash, out imageId))
            {
                return imageId;
            }

            var imageSize = ImagingUtilities.GetImageSize(imageData.Data);

            var manager = scope.KhumaloCraft.Image;

            var dalImage = new DalImage
            {
                InsertedOn = scope.ServerDate(),
                Size = dataLength,
                Hash = hash,
                ImageTypeId = (int)imageData.ContentType,
                Date = imageData.ModifiedDate,
                Width = (short)imageSize.Width,
                Height = (short)imageSize.Height,
                SavedToAzure = false
            };

            scope.KhumaloCraft.Image.Add(dalImage);

            scope.Commit();

            imageId = new ImageId(dalImage.Id);
        }

        _imageDataRepository.Write(imageId, imageData);

        using (var scope = DalScope.Begin())
        {
            var dalImage = scope.KhumaloCraft.Image.Single(i => i.Id == imageId.Value);

            dalImage.SavedToAzure = true;

            scope.KhumaloCraft.Image.Update(dalImage);

            scope.Commit();
        }

        return imageId;
    }

    public void ClaimOrphanedBlob(ImageId imageId, ImageData imageData)
    {
        using var scope = DalScope.Begin();

        if (scope.KhumaloCraft.Image.Any(i => i.Id == imageId.Value))
        {
            var dalImage = scope.KhumaloCraft.Image.Single(i => i.Id == imageId.Value);

            dalImage.SavedToAzure = true;

            scope.KhumaloCraft.Image.Update(dalImage);
        }
        else
        {
            var dataLength = imageData.Data.Length;

            var hash = ComputeHash(imageData);

            var imageSize = ImagingUtilities.GetImageSize(imageData.Data);

            //TODO : Implement ClaimOrphanedImageBlob
            //scope.KhumaloCraft.ClaimOrphanedImageBlob(imageId.Value, dataLength, hash, (int)imageData.ContentType, (short)imageSize.Width, (short)imageSize.Height);
        }

        scope.Commit();
    }

    private static bool TryFetch(IDalScope scope, int dataLength, byte[] hash, out ImageId imageId)
    {
        /*var dalImage = scope.KhumaloCraft.Image
                        .Where(i =>
                            i.Size == dataLength &&
                            i.Hash == hash &&
                            i.SavedToAzure)
                        .Order()
                        .FirstOrDefault();*/

        var dalImage = scope.KhumaloCraft.Image
                        .FirstOrDefault(i =>
                            i.Size == dataLength &&
                            i.Hash == hash &&
                            i.SavedToAzure);

        if (dalImage != null)
        {
            imageId = new ImageId(dalImage.Id);

            return true;
        }

        imageId = default;

        return false;
    }

    private static byte[] ComputeHash(ImageData imageData)
    {
        using var sha1 = SHA1.Create();

        return sha1.ComputeHash(imageData.Data);
    }

    public void DeleteImages(IEnumerable<ImageId> imageIds)
    {
        using var scope = DalScope.Begin();

        scope.KhumaloCraft.Image
                .RemoveAll(i => imageIds.Select(item => item.Value)
                .Contains(i.Id));

        foreach (var imageId in imageIds)
        {
            _imageDataRepository.Delete(imageId);
        }

        scope.Commit();
    }

    public List<ImageIdAndSize> FetchAllImagesSavedToAzureWithSize()
    {
        using var scope = DalScope.Begin();

        var images = scope.KhumaloCraft.Image
            .Where(i => i.SavedToAzure)
            .Select(i => new ImageIdAndSize(new ImageId(i.Id), i.Size));

        return images
            .OrderBy(image => image.Id.Value)
            .Distinct(image => image.Id.Value)
            .ToList();
    }
}