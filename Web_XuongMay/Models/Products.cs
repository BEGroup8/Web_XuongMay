using System;
using System.ComponentModel.DataAnnotations;

namespace Web_XuongMay.Models
{
    public class ProductsVM
    {
        [Required]
        public string TenHangHoa { get; set; }

        public string Mota { get; set; }

        [Required]
        public Guid MaLoai { get; set; }  // Khóa ngoại
    }
}
