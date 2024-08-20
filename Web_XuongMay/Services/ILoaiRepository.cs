using Web_XuongMay.Data;

public interface ILoaiRepository
{
    IEnumerable<Loai> GetAll();
    Loai GetById(Guid id);
    void Add(Loai loai);
    void Update(Loai loai);
    void Delete(Guid id);
}
