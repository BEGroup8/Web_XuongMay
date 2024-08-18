namespace Web_XuongMay.Models
{
    public class ProductsVM
    {
        public string TenHangHoa { get; set; }
        public string Mota { get; set; }
    }
    public class product : ProductsVM
    {
        public Guid MaHangHoa { get; set; }
    }
}
