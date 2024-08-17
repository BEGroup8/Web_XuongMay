namespace Web_XuongMay.Models
{
    public class CatagoryVM
    {
        public String Tenhang { get; set; }
        public double DonGia { get; set; }
    }

    public class Catagory : CatagoryVM
    {
        public Guid Mahh { get; set; }

    }
}

