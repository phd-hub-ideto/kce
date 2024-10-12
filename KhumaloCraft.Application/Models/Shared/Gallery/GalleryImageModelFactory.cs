using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;

namespace KhumaloCraft.Application.Models.Shared.Gallery;

[Singleton]
public sealed class GalleryImageModelFactory
{
    private readonly IImageUrlBuilder _imageUrlBuilder;

    public GalleryImageModelFactory(IImageUrlBuilder imageUrlBuilder)
    {
        _imageUrlBuilder = imageUrlBuilder;
    }

    public GalleryImageModel Create(
        IImage image,
        string alt,
        ImageSizeOption defaultSize = ImageSizeOption.Crop800x600,
        ImageSizeOption thumbnailSize = ImageSizeOption.Crop106x65,
        ImageSizeOption gridSize = ImageSizeOption.Crop328x246,
        ImageSizeOption originalSize = ImageSizeOption.Original)
    {
        return Create(image.ImageReferenceId, alt, defaultSize, thumbnailSize, gridSize, originalSize);
    }

    public GalleryImageModel Create(
      int imageReferenceId,
      string alt,
      ImageSizeOption defaultSize = ImageSizeOption.Crop800x600,
      ImageSizeOption thumbnailSize = ImageSizeOption.Crop106x65,
      ImageSizeOption gridSize = ImageSizeOption.Crop328x246,
      ImageSizeOption originalSize = ImageSizeOption.Original)
    {
        return new GalleryImageModel(
            imageId: imageReferenceId,
            altTag: alt,
            imageUrl: _imageUrlBuilder.GetUrl(imageReferenceId, defaultSize),
            thumbnailImageUrl: _imageUrlBuilder.GetUrl(imageReferenceId, thumbnailSize),
            gridImageUrl: _imageUrlBuilder.GetUrl(imageReferenceId, gridSize),
            originalImageUrl: _imageUrlBuilder.GetUrl(imageReferenceId, originalSize));
    }
}