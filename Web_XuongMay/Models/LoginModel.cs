using System.ComponentModel.DataAnnotations;

namespace Web_XuongMay.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(200)]
        public string Password { get; set; }
    }
}