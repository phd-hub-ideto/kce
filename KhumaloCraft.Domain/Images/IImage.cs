namespace KhumaloCraft.Domain.Images;

public interface IImage
{
    int ImageReferenceId { get; set; }
    DateTime? LastModified { get; set; }
    string Url { get; set; }
}