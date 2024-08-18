using Web_XuongMay.Data;

namespace Web_XuongMay.Models
{
    public class OrderProductModel
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public Products Product { get; set; }
    }
}
