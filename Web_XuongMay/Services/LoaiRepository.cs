using Web_XuongMay.Data;
using Web_XuongMay.Models;

namespace Web_XuongMay.Services
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly MyDbContext _context;

        public LoaiRepository(MyDbContext context)
        {
            _context = context;
        }

        public LoaiVM Add(LoaiModel loai)
        {
            var _loai = new Loai
            {
                MaLoai = Guid.NewGuid(), // Tạo Guid mới cho MaLoai
                TenLoai = loai.TenLoai,
            };
            _context.Add(_loai);
            _context.SaveChanges();
            return new LoaiVM
            {
                MaLoai = _loai.MaLoai,
                TenLoai = _loai.TenLoai,
            };
        }

        public void Delete(Guid id) // Đổi int thành Guid
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                _context.Remove(loai);
                _context.SaveChanges();
            }
        }

        public List<LoaiVM> GetAll()
        {
            var loais = _context.Loais.Select(x => new LoaiVM
            {
                MaLoai = x.MaLoai,
                TenLoai = x.TenLoai,
            });
            return loais.ToList();
        }

        public LoaiVM GetById(Guid id) // Đổi int thành Guid
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                return new LoaiVM
                {
                    MaLoai = loai.MaLoai,
                    TenLoai = loai.TenLoai
                };
            }
            return null;
        }

        public void Update(LoaiVM loai)
        {
            var _loai = _context.Loais.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if (_loai != null)
            {
                _loai.TenLoai = loai.TenLoai; // Cập nhật tên loại
                _context.SaveChanges();
            }
        }
    }
}
