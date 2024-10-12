using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("Order")]
public class DalOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    public int CartId { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    [Required]
    public double VatRate { get; set; }

    [Required]
    public int OrderStatusId { get; set; }

    [Required]
    public int LastEditedByUserId { get; set; }

    public DalCart Cart { get; set; }
    public DalOrderStatus OrderStatus { get; set; }
    public DalUser LastEditedByUser { get; set; }
}