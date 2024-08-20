using System.ComponentModel.DataAnnotations;

namespace Web_XuongMay.Data
{
    public class TaskOrder
    {
        [Key]
        public Guid TaskId { get; set; }

        public string TaskName { get; set; }

        public Guid ChuyenId { get; set; }
        public Chuyen Chuyen { get; set; }

        public Guid OrderProductId { get; set; }
        public OrderProduct OrderProduct { get; set; }
    }
}
