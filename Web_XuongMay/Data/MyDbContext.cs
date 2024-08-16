using Microsoft.EntityFrameworkCore;

namespace Web_XuongMay.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<Catagory> Catagories { get; set; }
         public DbSet<Loai> Loais { get; set; }
        #endregion

    }
}
