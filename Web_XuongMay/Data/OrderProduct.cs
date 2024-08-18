using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("OrderProducts")]
    public class OrderProduct
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        // Các thuộc tính điều hướng
        public virtual Order Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
