namespace Web_XuongMay.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; } // Nên mã hóa mật khẩu trước khi lưu
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
