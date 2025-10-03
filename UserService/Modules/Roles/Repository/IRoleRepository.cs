using UserService.Modules.Menus.Dto;
using UserService.Modules.Roles.Dto;

namespace UserService.Modules.Roles.Repository
{
    public interface IRoleRepository
    {
        public Task<List<string>> GetMenusByRoleIdAsync(string roleId);
        public Task AssignMenusToRoleAsync(string roleId, List<string> menuIds);
        public Task<List<RoleResultDto>> GetRolesWithMenus(List<string>? roleIds);
        public Task<int> SaveChangesAsync();
    }
}
