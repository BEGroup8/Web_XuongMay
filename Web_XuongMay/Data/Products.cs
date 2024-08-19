using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public Guid MaHH { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenMH { get; set; }

        public string MoTa { get; set; }

        public Guid MaLoai { get; set; }  // Khóa ngoại

        [ForeignKey(nameof(MaLoai))]
        public virtual Loai Loai { get; set; }  // Thuộc tính điều hướng
    }
}
