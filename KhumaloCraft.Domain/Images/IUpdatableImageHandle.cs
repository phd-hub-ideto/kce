namespace KhumaloCraft.Domain.Images;

public interface IUpdatableImageHandle
{
    void Commit(ImageReferenceAccumulator imageReferenceAccumulator);
}