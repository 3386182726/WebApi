using UserService.Modules.Menus.Dto;
using UserService.Modules.Menus.Model;
using UserService.Modules.Roles.Dto;
using UserService.Modules.Roles.Repository;

namespace UserService.Modules.Roles.Service
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        // 获取角色的菜单
        public async Task<List<string>> GetMenusByRoleIdAsync(string roleId)
        {
            return await roleRepository.GetMenusByRoleIdAsync(roleId);
        }

        public async Task<List<RoleResultDto>> GetRolesWithMenus(List<string>? roleIds)
        {
            return await roleRepository.GetRolesWithMenus(roleIds);
        }

        // 分配菜单给角色
        public async Task<int> AssignMenusToRoleAsync(string roleId, List<string> menuIds)
        {
            await roleRepository.AssignMenusToRoleAsync(roleId, menuIds);
            return await roleRepository.SaveChangesAsync();
        }
    }
}
