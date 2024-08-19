using System.ComponentModel.DataAnnotations.Schema;
using Web_XuongMay.Data;

namespace Web_XuongMay.Models
{
    public class OrderModel
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderProductModel> OrderProducts { get; set; }
    }
}
