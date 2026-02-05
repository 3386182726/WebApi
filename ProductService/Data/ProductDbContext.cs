using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProductService.Modules.Products.Model;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=.;Database=product;User Id=sa;Password=sa;TrustServerCertificate=True;"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<Product>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()"); // SQL Server 上生成 Guid

            builder
                .Entity<Product>()
                .Property(p => p.Imgs)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v =>
                        JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)
                        ?? Array.Empty<string>()
                );
        }
    }
}
