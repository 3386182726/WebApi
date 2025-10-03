using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Modules.Roles.Dto;
using UserService.Modules.Roles.Service;
using UserService.Modules.Users.Repository;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Roles
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize(Roles = "Admin")] // 只有管理员可以管理角色
    public class RoleController(RoleManager<IdentityRole> roleManager, IRoleService roleService)
        : ControllerBase
    {
        [HttpPost("save")]
        public async Task<IActionResult> SaveRole([FromBody] RolePostDto role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(role.Id))
            {
                // 创建角色
                if (await roleManager.RoleExistsAsync(role.Name))
                    return BadRequest("角色已存在");

                var result = await roleManager.CreateAsync(new IdentityRole(role.Name));
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }
            else
            {
                // 修改角色
                var existingRole = await roleManager.FindByIdAsync(role.Id);
                if (existingRole == null)
                    return NotFound("角色不存在");

                existingRole.Name = role.Name;
                var result = await roleManager.UpdateAsync(existingRole);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            return Ok("操作成功");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound("角色不存在");

            // 删除角色
            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("角色已删除");
        }

        [HttpGet("all")]
        public IActionResult GetRoles()
        {
            var result = roleManager.Roles.ToList();

            return Ok(result);
        }

        [HttpGet("all/menus")]
        public async Task<IActionResult> GetRolesWithMenus([FromBody] List<string>? roleIds)
        {
            var result = await roleService.GetRolesWithMenus(roleIds);
            return Ok(result);
        }

        // 分配菜单给角色
        [HttpPost("{roleId}/menus")]
        public async Task<IActionResult> AssignMenusToRole(
            string roleId,
            [FromBody] List<string> menuIds
        )
        {
            var result = await roleService.AssignMenusToRoleAsync(roleId, menuIds);
            if (result == 0)
            {
                return NoContent(); // 204 No Content 表示操作成功且没有返回内容
            }
            return Ok("分配菜单成功");
        }
    }
}
