using UserService.Modules.Menus.Model;

namespace UserService.Modules.Menus.Repository
{
    public interface IMenuRepository
    {
        public Task<Menu?> GetAsync(string id);
        public Task<List<Menu>> GetListAsync();
        public void Create(Menu menu);
        public void Update(Menu menu);
        public Task<int> SaveChangesAsync();
    }
}
