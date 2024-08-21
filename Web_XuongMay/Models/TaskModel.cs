namespace Web_XuongMay.Models
{
    public class TaskModel
    {
        public string TaskName { get; set; }    // Tên của Task
        public Guid ChuyenId { get; set; }      // Khóa ngoại liên kết đến Chuyen
        public Guid OrderProductId { get; set; } // Khóa ngoại liên kết đến OrderProduct
    }

}
