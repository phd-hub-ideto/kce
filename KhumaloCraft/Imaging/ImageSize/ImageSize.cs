namespace KhumaloCraft.Imaging;

public struct ImageSize
{
    public static readonly ImageSize Original = new ImageSize();

    public ImageResizeOption? ResizeOption { get; internal set; }
    public uint Width { get; }
    public uint Height { get; }

    public ImageSize(uint width, uint height)
        : this()
    {
        Width = width;
        Height = height;
        ResizeOption = null;
    }
    public ImageSize(System.Drawing.Size drawingSize)
        : this((uint)drawingSize.Width, (uint)drawingSize.Height)
    {
    }
    internal ImageSize(ImageResizeOption resizeOption, uint width, uint height)
        : this()
    {
        Width = width;
        Height = height;
        ResizeOption = resizeOption;
    }

    public override bool Equals(object obj)
    {
        if (null == obj || !(obj is ImageSize))
            return false;
        ImageSize size = (ImageSize)obj;
        return size.ResizeOption == ResizeOption && size.Width == Width && size.Height == Height;
    }

    public override int GetHashCode()
    {
        return ResizeOption.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
    }

    public override string ToString()
    {
        return
            (ResizeOption.HasValue ? ResizeOption.Value.ToString() : string.Empty) +
            Width.ToString() + "x" + Height.ToString();
    }
}