using Web_XuongMay.Models;

namespace Web_XuongMay.Services
{
    public interface ILoaiRepository
    {
        List<LoaiVM> GetAll();
        LoaiVM GetById(Guid id);
        LoaiVM Add(LoaiModel loai);
        void Update(LoaiVM loai);
        void Delete(Guid id);
    }
}
