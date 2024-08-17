using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Catagory")]
    public class Catagory
    {
        [Key]
        public Guid Mahh { get; set; }
        [Required]
        [MaxLength(50)]
        public string Tenhang { get; set; }
        public string Mota { get; set; }
        [Range(0, double.MaxValue)]  
        public double DonGia { get; set; }

        public int? MaLoai { get; set; }
        [ForeignKey("MaLoai")]
        public Loai Loai { get; set; }
    }
}
