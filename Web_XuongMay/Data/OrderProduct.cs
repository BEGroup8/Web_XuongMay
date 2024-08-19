using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web_XuongMay.Data;

public class OrderProduct
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; }

    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Products Product { get; set; }

    public int Quantity { get; set; }
}
