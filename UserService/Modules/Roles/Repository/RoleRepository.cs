using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Menus.Dto;
using UserService.Modules.Menus.Model;
using UserService.Modules.Roles.Dto;

namespace UserService.Modules.Roles.Repository
{
    public class RoleRepository(AppDbContext appDbContext) : IRoleRepository
    {
        public async Task<List<string>> GetMenusByRoleIdAsync(string roleId)
        {
            return await appDbContext
                .RoleMenus.Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.MenuId)
                .ToListAsync();
        }

        public async Task AssignMenusToRoleAsync(string roleId, List<string> menuIds)
        {
            var existingEntries = await appDbContext
                .RoleMenus.Where(rm => rm.RoleId == roleId)
                .ToListAsync();

            appDbContext.RoleMenus.RemoveRange(existingEntries); // 清除之前的所有关联

            var roleMenus = menuIds
                .Select(menuId => new RoleMenu { RoleId = roleId, MenuId = menuId })
                .ToList();

            await appDbContext.RoleMenus.AddRangeAsync(roleMenus);
        }

        public async Task<List<RoleResultDto>> GetRolesWithMenus(List<string>? roles)
        {
            var rolesWithMenus = await (
                from role in appDbContext.Roles
                where roles == null || roles.Count == 0 || roles.Contains(role.Name!)
                join rm in appDbContext.RoleMenus on role.Id equals rm.RoleId into roleMenus
                from rm in roleMenus.DefaultIfEmpty() // 左连接 RoleMenu
                join menu in appDbContext.Menus on rm.MenuId equals menu.Id into menus
                from menu in menus.DefaultIfEmpty() // 左连接 Menu
                select new
                {
                    Id = role.Id,
                    Name = role.Name!,
                    Menu = new MenuResultDto
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Path = menu.Path,
                    },
                }
            )
                .GroupBy(x => new { x.Id, x.Name })
                .Select(g => new RoleResultDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Menus = g.Select(x => new MenuResultDto
                        {
                            Id = x.Menu.Id,
                            Name = x.Menu.Name,
                            Path = x.Menu.Path,
                        })
                        .ToList(),
                })
                .ToListAsync();

            return rolesWithMenus;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }
    }
}
