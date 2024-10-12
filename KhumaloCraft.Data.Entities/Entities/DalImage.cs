using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("Image")]
public class DalImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Size { get; set; }

    [Required]
    public int ImageTypeId { get; set; }

    [Required]
    public int Width { get; set; }

    [Required]
    public int Height { get; set; }

    [Required]
    public byte[] Hash { get; set; }

    [Required]
    public DateTime InsertedOn { get; set; }

    [Required]
    public bool SavedToAzure { get; set; }

    public DalImageType ImageType { get; set; }
}