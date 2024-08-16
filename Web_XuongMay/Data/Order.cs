using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Orders")]
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Sử dụng IEnumerable để biểu diễn tập hợp dữ liệu
        public virtual IEnumerable<OrderProduct> OrderProducts { get; set; }
    }
}
