using Microsoft.EntityFrameworkCore;
using Web_XuongMay.Models;

namespace Web_XuongMay.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Loai> Loais { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Chuyen> Chuyens { get; set; }
        public DbSet<TaskOrder> TaskOrders { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderId)
                .HasColumnName("OrderId");

            modelBuilder.Entity<Products>()
                .Property(p => p.MaHH)
                .HasColumnName("MaHH");

            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => op.Id); 

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany() 
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");
            });
        }

    }
}
