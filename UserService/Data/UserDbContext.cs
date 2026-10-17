using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Modules.Organizations;
using UserService.Modules.Menus.Model;
using UserService.Modules.Users;

namespace UserService.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options)
        : IdentityDbContext<User, IdentityRole, string>(options)
    {
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<Menu>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder
                .Entity<RoleMenu>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder
                .Entity<Organization>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");
        }
    }
}
