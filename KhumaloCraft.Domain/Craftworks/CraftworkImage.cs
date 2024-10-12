using KhumaloCraft.Domain.Images;

namespace KhumaloCraft.Domain.Craftworks;

[Serializable]
public class CraftworkImage : IImage
{
    public int ImageReferenceId { get; set; }
    public DateTime? LastModified { get; set; }
    public string Url { get; set; }
}