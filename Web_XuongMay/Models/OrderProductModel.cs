using System;
using Web_XuongMay.Data;

namespace Web_XuongMay.Models
{
    public class OrderProductModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Products Product { get; set; } // Optionally include product details
    }
}
