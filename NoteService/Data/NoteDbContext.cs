using Microsoft.EntityFrameworkCore;
using NoteService.Modules.Notes.Model;

namespace NoteService.Data
{
    public class NoteDbContext: DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteCategory> NoteCategories { get; set; }

        public DbSet<User> Users { get; set; }
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<Note>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()"); // SQL Server 上生成 Guid

            builder
                .Entity<NoteCategory>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()"); // SQL Server 上生成 Guid
            builder
               .Entity<User>()
               .Property(m => m.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("NEWID()"); // SQL Server 上生成 Guid
        }
    }
}
