namespace KhumaloCraft.Domain.Images;

public struct ImageIdAndSize
{
    public ImageId Id { get; }
    public int Size { get; }

    public ImageIdAndSize(ImageId id, int size)
    {
        Id = id;
        Size = size;
    }

    public override bool Equals(object obj)
    {
        return obj is ImageIdAndSize other &&
               EqualityComparer<ImageId>.Default.Equals(Id, other.Id) &&
               Size == other.Size;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Size);
    }

    public void Deconstruct(out ImageId id, out int size)
    {
        id = Id;
        size = Size;
    }

    public static implicit operator (ImageId Id, int Size)(ImageIdAndSize value)
    {
        return (value.Id, value.Size);
    }

    public static implicit operator ImageIdAndSize((ImageId Id, int Size) value)
    {
        return new ImageIdAndSize(value.Id, value.Size);
    }

    public override string ToString()
    {
        return $"(Id:{Id}, Size={Size})";
    }
}