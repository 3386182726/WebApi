using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Menus.Model;

namespace UserService.Modules.Menus.Repository
{
    public class MenuRepository(AppDbContext appDbContext) : IMenuRepository
    {
        public async Task<Menu?> GetAsync(string id)
        {
            return await appDbContext.Menus.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Menu>> GetListAsync()
        {
            return await appDbContext.Menus.ToListAsync();
        }

        public void Create(Menu menu)
        {
            appDbContext.Menus.Add(menu);
        }

        public void Update(Menu menu)
        {
            appDbContext.Menus.Update(menu);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }
    }
}
