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
using UserService.Migrations;
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
        public async Task<IActionResult> Login2(LoginRequest dto)
        {
            var user = await userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized();

            await signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new { message = "登录成功" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest dto)
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
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            Console.WriteLine($"用户开始登录：{dto.Username}");
            var user = await userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized();
            Console.WriteLine($"已查找到用户：{dto.Username}");
            var roles = await userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName!) };
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
        public async Task<IActionResult> CreateUser(CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user;

            if (!string.IsNullOrEmpty(dto.Id))
            {
                user = await userManager.FindByIdAsync(dto.Id);
                if (user == null)
                    return NotFound("用户不存在");

                user.UserName = dto.UserName;
                user.Name = dto.Name;
                user.Introduction = dto.Introduction;
                user.Email = dto.Email;
                user.OrganizationId = dto.OrganizationId;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return BadRequest(updateResult.Errors);
            }
            else
            {
                user = new User
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Name = dto.Name,
                    Introduction = dto.Introduction,
                    OrganizationId = dto.OrganizationId,
                };

                var password = configuration["DefaultUserPassword"]!;
                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                    return BadRequest(createResult.Errors);
            }

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
                IsDeleted = false
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

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized(new { message = "未登录" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await userService.GetCurrentUserAsync(userId);
            if (user == null)
                return NotFound("用户不存在");

            user.RolesWithMenus = await roleService.GetRolesWithMenus(user.Roles);

            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsers([FromQuery] PagedRequest request)
        {
            var users = await userService.GetUsersAsync(request);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
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
                UserName = user?.UserName ?? string.Empty,
                Name = user?.Name,
                IsDeleted = true
            };
            await userService.PublishUserUpsertEventAsync(userUpsertEvent);
            return Ok("删除成功");
        }
    }
}
