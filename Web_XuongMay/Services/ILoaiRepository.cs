using Web_XuongMay.Data;
using Web_XuongMay.Models;
using System.Collections.Generic;

public interface ILoaiRepository
{
    List<LoaiVM> GetAll();
    LoaiVM GetById(Guid id);
    LoaiVM Add(LoaiModel loai);
    void Update(LoaiVM loai);
    void Delete(Guid id);
    
    void Add(Loai loai);
}
