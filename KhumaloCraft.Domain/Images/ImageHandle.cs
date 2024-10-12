using KhumaloCraft.Domain.Images;

namespace KhumaloCraft.Domain;

[Serializable]
public class ImageHandle : IUpdatableImageHandle
{
    private ImageHandle(int? imageReferenceId)
    {
        _currentImageReferenceId = imageReferenceId;
    }

    private int? _removedImageReferenceId;
    private int? _currentImageReferenceId;
    private bool _updated = false;

    public static ImageHandle Create()
    {
        return new ImageHandle(null);
    }

    internal static ImageHandle CreateFromReference(int? imageReferenceId)
    {
        return new ImageHandle(imageReferenceId);
    }

    public int? ReferenceId
    {
        get
        {
            if (_updated)
            {
                throw new InvalidOperationException($"{nameof(ImageHandle)} must be commited before the reference can be used.");
            }

            return _currentImageReferenceId;
        }
    }

    public bool HasValue => _currentImageReferenceId != null;

    public void SetReferenceId(int? imageReferenceId)
    {
        if (_currentImageReferenceId == imageReferenceId) return;

        // If this is the first call, back up the old value so that we can mark it as deleted later.
        if (!_updated)
        {
            _removedImageReferenceId = _currentImageReferenceId;
            _updated = true;
        }

        _currentImageReferenceId = imageReferenceId;
    }

    void IUpdatableImageHandle.Commit(ImageReferenceAccumulator imageReferenceAccumulator)
    {
        if (_removedImageReferenceId != null)
        {
            imageReferenceAccumulator.Remove(_removedImageReferenceId.Value);
        }

        if (_updated && _currentImageReferenceId != null)
        {
            imageReferenceAccumulator.Add(_currentImageReferenceId.Value);
        }

        _removedImageReferenceId = null;

        _updated = false;
    }

    internal int? SaveToImageServer()
    {
        var imageManager = Dependencies.DependencyManager.Container.GetInstance<IImageReferenceManager>();

        imageManager.PersistReference(this);

        return ReferenceId;
    }
}