using CategoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Data
{
    public class CategoryDbContext(DbContextOptions options): DbContext(options)
    {
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryGuid).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()").HasColumnType("datetime2(0)");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()").HasColumnType("datetime2(0)");
            });

        }
    }
}
