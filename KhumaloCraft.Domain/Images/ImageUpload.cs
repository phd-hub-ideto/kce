namespace KhumaloCraft.Domain.Images;

public class ImageUpload
{
    public byte[] Data { get; }

    public DateTime? ModifiedDate { get; }

    public ImageUpload()
    {

    }

    public ImageUpload(byte[] data, DateTime? modifiedDate = null)
    {
        Data = data;
        ModifiedDate = modifiedDate;
    }
}