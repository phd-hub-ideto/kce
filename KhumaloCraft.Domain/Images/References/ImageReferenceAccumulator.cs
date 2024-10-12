namespace KhumaloCraft.Domain.Images;

public class ImageReferenceAccumulator
{
    public HashSet<int> Removed { get; } = new HashSet<int>();
    public HashSet<int> Added { get; } = new HashSet<int>();

    internal void Remove(int imageReferenceId)
    {
        if (!Added.Remove(imageReferenceId))
        {
            Removed.Add(imageReferenceId);
        }
    }

    internal void Add(int imageReferenceId)
    {
        if (!Removed.Remove(imageReferenceId))
        {
            Added.Add(imageReferenceId);
        }
    }
}