using System.ComponentModel.DataAnnotations;

namespace Web_XuongMay.Models
{
    public class LoaiModel
    {
        [Required]
        [MaxLength(50)]
        public string TenLoai { get; set; }
    }
}
