using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Tiff;

namespace KhumaloCraft.Imaging;

public static class ImagingUtilities
{
    public static ImageSize GetImageSize(byte[] imageData)
    {
        using var image = Image.Load(imageData);

        return new ImageSize((uint)image.Width, (uint)image.Height);
    }

    public static byte[] EnsureImage(byte[] imageData, uint width, uint height, ImageResizeOption? resizeOption)
    {
        using var image = Image.Load(imageData);

        IImageFormat format = image.Metadata.DecodedImageFormat;

        switch (resizeOption)
        {
            case null:
                return image.SaveImage();
            case ImageResizeOption.Ensure:
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size((int)width, (int)height),
                    Mode = ResizeMode.Max
                }));
                break;

            case ImageResizeOption.FitInside:
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size((int)width, (int)height),
                    Mode = ResizeMode.Pad
                }));
                break;

            case ImageResizeOption.Crop:
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size((int)width, (int)height),
                    Mode = ResizeMode.Crop
                }));
                break;

            case ImageResizeOption.EnsurePanoramic:
                // TODO-LP Implement panoramic logic if needed
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(resizeOption), "Invalid resize option.");
        }

        return image.SaveImage();
    }

    public static byte[] SaveImage(this Image image)
    {
        IImageFormat format = image.Metadata.DecodedImageFormat;

        using var ms = new MemoryStream();

        if (format is PngFormat)
        {
            image.SaveAsPng(ms);
        }
        else if (format is JpegFormat)
        {
            image.SaveAsJpeg(ms);
        }
        else if (format is GifFormat)
        {
            image.SaveAsGif(ms);
        }
        else if (format is BmpFormat)
        {
            image.SaveAsBmp(ms);
        }
        else if (format is TiffFormat)
        {
            image.SaveAsTiff(ms);
        }
        else
        {
            throw new NotSupportedException($"Unsupported image format: {format}.");
        }

        return ms.ToArray();
    }

    public static ImageContentType GetImageContentType(byte[] imageData)
    {
        using var image = Image.Load(imageData);

        // Detect the format of the image
        IImageFormat format = image.Metadata.DecodedImageFormat;

        return format switch
        {
            JpegFormat _ => ImageContentType.Jpeg,
            PngFormat _ => ImageContentType.Png,
            GifFormat _ => ImageContentType.Gif,
            TiffFormat _ => ImageContentType.Tiff,
            BmpFormat _ => ImageContentType.Bmp,
            null => ImageContentType.Missing,
            _ => throw new NotSupportedException($"Image Content Type ({format}) not supported")
        };
    }
}