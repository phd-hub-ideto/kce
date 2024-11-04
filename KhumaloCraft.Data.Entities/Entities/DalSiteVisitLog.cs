using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("SiteVisitLog")]
public class DalSiteVisitLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime DateVisited { get; set; }
    public string RequestPath { get; set; }
    public string ContentType { get; set; }
    public string Scheme { get; set; }
    public string Referrer { get; set; }

    [Required]
    [MaxLength(50)]
    public string Method { get; set; }
    public string Host { get; set; }
    public int StatusCode { get; set; }
    public string Location { get; set; }
    public string UserAgent { get; set; }

    [MaxLength(50)]
    public string Platform { get; set; }
    public string IpAddress { get; set; }
    public int? UserId { get; set; }

    [MaxLength(50)]
    public string UniqueUserId { get; set; }
    public DalUser VisitUser { get; set; }
}