using System.ComponentModel.DataAnnotations.Schema;
using Web_XuongMay.Data;

namespace Web_XuongMay.Models
{
    public class OrderModel
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Sử dụng IEnumerable để biểu diễn tập hợp dữ liệu
        public virtual IEnumerable<OrderProduct> OrderProducts { get; set; }
    }
}
