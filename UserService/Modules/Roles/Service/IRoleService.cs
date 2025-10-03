using UserService.Modules.Menus.Dto;
using UserService.Modules.Roles.Dto;

namespace UserService.Modules.Roles.Service
{
    public interface IRoleService
    {
        public Task<List<string>> GetMenusByRoleIdAsync(string roleId);
        public Task<List<RoleResultDto>> GetRolesWithMenus(List<string>? roleIds);
        Task<int> AssignMenusToRoleAsync(string roleId, List<string> menuIds);
    }
}
