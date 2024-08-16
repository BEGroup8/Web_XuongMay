using Microsoft.EntityFrameworkCore;

namespace Web_XuongMay.Data
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }
        #region DbSet
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e=> e.UserName).IsUnique();
                entity.Property(e=> e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);

            });
        }
        
        
        #endregion

    }
}
