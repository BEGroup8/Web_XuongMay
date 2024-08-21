using System.ComponentModel.DataAnnotations;

namespace Web_XuongMay.Models
{
    public class LineModel
    {
        [Required]
        [MaxLength(50)]
        public string NameLine { get; set; }

        [Required]  // Đảm bảo rằng UserId là bắt buộc
        public int UserId { get; set; }
    }

}