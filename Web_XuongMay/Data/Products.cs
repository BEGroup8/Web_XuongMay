using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("ProDucts")]
    public class Products
    {
        [Key]
        public  Guid MaHH { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenMH { get; set; }

        public string MoTa { get; set;}


    }
}
