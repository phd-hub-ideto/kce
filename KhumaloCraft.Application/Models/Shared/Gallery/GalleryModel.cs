namespace KhumaloCraft.Application.Models.Shared.Gallery;

public class GalleryModel
{
    public List<GalleryImageModel> GalleryImages { get; set; }
    public string FullscreenTitle { get; }

    public bool? EnableFullWidthPictures { get; set; }

    public GalleryModel(
        string fullscreenTitle,
        bool? enableFullWidthPictures = null)
    {
        FullscreenTitle = fullscreenTitle;
        EnableFullWidthPictures = enableFullWidthPictures;
    }
}