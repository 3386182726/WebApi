using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Modules.Role.Dto;
using WebApi.Modules.User.Repository;
using WebApi.Modules.User.Service;

namespace WebApi.Modules.Role
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 只有管理员可以管理角色
    public class RoleController(RoleManager<IdentityRole> roleManager, IUserService userService)
        : ControllerBase
    {
        [HttpPost("save")]
        public async Task<IActionResult> SaveRole([FromBody] RoleDto role)
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
        public IActionResult GetUsers([FromQuery] PagedRequest request)
        {
            var result = userService.GetUsersAsync(request);

            return Ok(result);
        }
    }
}
