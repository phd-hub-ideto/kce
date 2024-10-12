using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("User")]
public class DalUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Username { get; set; }

    [Required]
    [MaxLength(150)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(150)]
    public string LastName { get; set; }

    [MaxLength(15)]
    public string MobileNumber { get; set; }

    public int? ImageReferenceId { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }

    public DateTime? ActivatedDate { get; set; }
    public DateTime? ActivationEmailSentDate { get; set; }
    public DateTime? LastLoginDate { get; set; }

    [Required]
    public byte[] PasswordHash { get; set; }

    [Required]
    public byte[] PasswordSalt { get; set; }

    [Required]
    public bool ValidatedEmail { get; set; }

    [Required]
    public bool Deleted { get; set; }
}