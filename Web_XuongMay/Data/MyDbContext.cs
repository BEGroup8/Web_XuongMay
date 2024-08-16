using Microsoft.EntityFrameworkCore;

namespace Web_XuongMay.Data
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Products> products { get; set; }
    }
}
