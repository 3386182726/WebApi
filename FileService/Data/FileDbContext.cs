using Microsoft.EntityFrameworkCore;
using FileService.Modules.Files;

namespace FileService.Data
{
    public class FileDbContext(DbContextOptions<FileDbContext> options) : DbContext(options)
    {
        public DbSet<AppFile> AppFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<AppFile>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");
        }
    }
}