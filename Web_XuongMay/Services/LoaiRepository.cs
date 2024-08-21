using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Loai> GetAll()
        {
            return _context.Loais.ToList();
        }

        public Loai GetById(Guid id)
        {
            return _context.Loais.SingleOrDefault(x => x.MaLoai == id);
        }

        public void Add(Loai loai)
        {
            _context.Loais.Add(loai);
            _context.SaveChanges();
        }

        public void Update(Loai loai)
        {
            var existingLoai = _context.Loais.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if (existingLoai != null)
            {
                existingLoai.TenLoai = loai.TenLoai;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                _context.Loais.Remove(loai);
                _context.SaveChanges();
            }
        }
    }
}
