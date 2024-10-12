namespace KhumaloCraft.Domain.Images;

[Serializable]
public struct ImageId
{
    public ImageId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public override bool Equals(object obj)
    {
        return obj is ImageId id
            && Value == id.Value;
    }

    public override int GetHashCode()
    {
        return -1937169414 + Value.GetHashCode();
    }

    internal static ImageId? Create(int? imageId)
    {
        if (imageId.HasValue)
        {
            return new ImageId(imageId.Value);
        }
        return null;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
