using Common.Models;
using UserService.Modules.Roles.Dto;

namespace UserService.Modules.Users.Dto;

public class UserResponse
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? Name { get; set; }
    public string? Introduction { get; set; }
    public DateTime? Birthday { get; set; }
    public string? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public List<string> Roles { get; set; } = [];

    /// <summary>带菜单的角色详情（仅 /me 端点填充）</summary>
    public List<RoleResponse>? RolesWithMenus { get; set; }

    // 聚合的图片，按 Type 分开
    public List<AppFileResponse> Avatars { get; set; } = [];
    public List<AppFileResponse> Photos { get; set; } = [];
}
