using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Web_XuongMay.Services;

namespace Web_XuongMay.Services
{
public class LoaiRepository : ILoaiRepository
{
    private readonly MyDbContext _context;

    public LoaiRepository(MyDbContext context)
    {
        _context = context;
    }

    public List<LoaiVM> GetAll()
    {
        return _context.Loais
            .Select(loai => new LoaiVM
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai
            })
            .ToList();
    }

    public LoaiVM GetById(Guid id)
    {
        var loai = _context.Loais
            .Where(l => l.MaLoai == id)
            .Select(l => new LoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai
            })
            .FirstOrDefault();

        return loai;
    }

    public LoaiVM Add(LoaiModel loaiModel)
    {
        var loai = new Loai
        {
            MaLoai = Guid.NewGuid(),
            TenLoai = loaiModel.TenLoai
        };

        _context.Loais.Add(loai);
        _context.SaveChanges();

        return new LoaiVM
        {
            MaLoai = loai.MaLoai,
            TenLoai = loai.TenLoai
        };
    }
            return null;
        }

    public void Update(LoaiVM loaiVM)
    {
        var loai = _context.Loais.Find(loaiVM.MaLoai);
        if (loai != null)
        {
            loai.TenLoai = loaiVM.TenLoai;
            _context.SaveChanges();
        }
    }

    public void Delete(Guid id)
    {
        var loai = _context.Loais.Find(id);
        if (loai != null)
        {
            _context.Loais.Remove(loai);
            _context.SaveChanges();
        }
    }
}
