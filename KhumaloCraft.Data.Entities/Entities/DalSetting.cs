using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("Setting")]
public class DalSetting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Value { get; set; }

    [Required]
    public int LastEditedByUserId { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    public DalUser LastEditedByUser { get; set; }
}