using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Data
{
    public class DictionaryDbContext(DbContextOptions<DictionaryDbContext> options) : DbContext(options)
    {
        public DbSet<Modules.Dictionaries.Dictionary> Dictionaries => Set<Modules.Dictionaries.Dictionary>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Modules.Dictionaries.Dictionary>(entity =>
            {
                entity.Property(d => d.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(d => new { d.DictType, d.Code }).IsUnique();
                entity.HasIndex(d => d.DictType);
            });
        }
    }
}
