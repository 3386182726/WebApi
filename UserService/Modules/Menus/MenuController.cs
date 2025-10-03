using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Modules.Menus.Dto;
using UserService.Modules.Menus.Model;
using UserService.Modules.Menus.Repository;
using UserService.Modules.Menus.Service;

namespace UserService.Modules.Menus
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class MenuController(IMenuRepository menuRepository) : ControllerBase
    {
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetMenu()
        {
            var menus = await menuRepository.GetListAsync();
            var tree = MenuMapper.BuildTree(menus);
            return Ok(tree);
        }

        [HttpPost("save")]
        [Authorize]
        public async Task<IActionResult> SaveMenu(MenuPostDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
            {
                var menu = await menuRepository.GetAsync(dto.Id);
                if (menu == null)
                    return NotFound("菜单不存在");
                menu.Name = dto.Name;
                menu.Path = dto.Path;
                menu.ParentId = dto.ParentId;
                menuRepository.Update(menu);
            }
            else
            {
                var menu = new Menu()
                {
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    Path = dto.Path,
                };
                menuRepository.Create(menu);
            }
            await menuRepository.SaveChangesAsync();
            return Ok("菜单保存成功");
        }
    }
}
