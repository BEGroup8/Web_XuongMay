using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Loai")]
    public class Loai
    {
        [Key]
        public Guid MaLoai { get; set; }  // Sử dụng Guid làm khóa chính

        [Required]
        [MaxLength(50)]
        public string TenLoai { get; set; }

    }
}
