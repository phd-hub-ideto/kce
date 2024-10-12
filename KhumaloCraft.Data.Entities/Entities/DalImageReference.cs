using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("ImageReference")]
public class DalImageReference
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReferenceId { get; set; }

    public int? ImageId { get; set; }

    public DateTime? DeletedOn { get; set; }

    public DalImage Image { get; set; }
}