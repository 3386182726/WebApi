using Common.Models;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Organizations.Dto;

public class OrganizationResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<AppFileResponse> Avatars { get; set; } = [];
    public string? ParentId { get; set; }

    /// <summary>组织下的人员（带头像、角色、人名）</summary>
    public List<UserResponse> Users { get; set; } = [];

    public List<OrganizationResponse> Children { get; set; } = [];
}
