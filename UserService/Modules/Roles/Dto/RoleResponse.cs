using System.ComponentModel.DataAnnotations;
using UserService.Modules.Menus.Dto;

namespace UserService.Modules.Roles.Dto;

public class RoleResponse
{
    public required string Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public List<MenuResponse> Menus { get; set; } = [];
}
