using System.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Modules.User.Dto;
using WebApi.Modules.User.Service;

namespace WebApi.Modules.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    ) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 验证用户名和密码
            var user = await userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized();

            await signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new { message = "登录成功" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User() { UserName = dto.UserName, Email = dto.Email };
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("注册成功");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "已登出" });
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] LoginDto login)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "已登出" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized(new { message = "未登录" });

            var user = await userManager.GetUserAsync(User);
            return Ok(
                new
                {
                    username = user!.UserName,
                    email = user.Email,
                    roles = await userManager.GetRolesAsync(user),
                }
            );
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsers([FromQuery] PagedRequest request)
        {
            var users = await userService.GetUsersAsync(request);
            return Ok(users);
        }
    }
}
