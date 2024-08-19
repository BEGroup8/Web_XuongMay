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

        public LoaiVM Add(LoaiModel loaiModel)
        {
            var loai = new Loai
            {
                MaLoai = Guid.NewGuid(),  // Tạo GUID mới cho MaLoai
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

        public void Add(Loai loai)
        {
            // Phương thức này không được sử dụng trong mã hiện tại
            throw new NotImplementedException();
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

        public List<LoaiVM> GetAll()
        {
            var loais = _context.Loais.Select(x => new LoaiVM
            {
                MaLoai = x.MaLoai,
                TenLoai = x.TenLoai
            });

            return loais.ToList();
        }

        public LoaiVM GetById(Guid id)
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

        public void Update(LoaiVM loaiVm)
        {
            var loai = _context.Loais.SingleOrDefault(x => x.MaLoai == loaiVm.MaLoai);
            if (loai != null)
            {
                loai.TenLoai = loaiVm.TenLoai;
                _context.SaveChanges();
            }
        }
    }
}
