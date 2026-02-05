using Common.Pagination;
using Contracts.Event.User;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Modules.Roles.Service;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService,
        IRoleService roleService,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration
    ) : ControllerBase
    {
        [HttpPost("login2")]
        public async Task<IActionResult> Login2(LoginDto dto)
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new User() { UserName = dto.UserName, Email = dto.Email };
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var userUpsertEvent = new UserUpsertEvent
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user?.Name,
                IsDeleted = false
            };
            await userService.PublishUserUpsertEventAsync(userUpsertEvent);
            return Ok("注册成功");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized();
            var roles = await userManager.GetRolesAsync(user);
            // 生成 JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id) };
            // 循环添加角色
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    double.Parse(configuration["Jwt:ExpireMinutes"]!)
                ),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user;

            if (!string.IsNullOrEmpty(dto.Id)) // 更新用户
            {
                user = await userManager.FindByIdAsync(dto.Id);
                if (user == null)
                    return NotFound("用户不存在");

                // 更新基本信息
                user.UserName = dto.UserName;
                user.Name = dto.Name;
                user.Email = dto.Email;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return BadRequest(updateResult.Errors);
            }
            else // 新建用户
            {
                user = new User { UserName = dto.UserName, Email = dto.Email };

                var password = configuration["DefaultUserPassword"]!;
                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                    return BadRequest(createResult.Errors);
            }

            // 添加角色（如果有）
            if (dto.Roles != null && dto.Roles.Count > 0)
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return BadRequest(removeResult.Errors);
                }

                var addRolesResult = await userManager.AddToRolesAsync(user, dto.Roles);
                if (!addRolesResult.Succeeded)
                    return BadRequest(addRolesResult.Errors);
            }

            var userUpsertEvent = new UserUpsertEvent
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user?.Name,
                IsDeleted =false
            };
            await userService.PublishUserUpsertEventAsync(userUpsertEvent);

            return Ok("成功");
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
            var roles = await userManager.GetRolesAsync(user!);
            var rolesWithMenus = await roleService.GetRolesWithMenus(roles.ToList());
            return Ok(
                new
                {
                    name = user!.Name,
                    username = user!.UserName,
                    email = user.Email,
                    roles = rolesWithMenus,
                }
            );
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsers([FromQuery] PagedRequest request)
        {
            var users = await userService.GetUsersAsync(request);
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("用户不存在");
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "删除失败", errors });
            }
            var userUpsertEvent = new UserUpsertEvent
            {
                UserId = user.Id,
                UserName = user?.UserName??string.Empty,
                Name = user?.Name,
                IsDeleted = true
            };
            await userService.PublishUserUpsertEventAsync(userUpsertEvent);
            return Ok("删除成功");
        }
    }
}
