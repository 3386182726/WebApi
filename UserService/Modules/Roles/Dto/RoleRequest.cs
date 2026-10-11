using System.ComponentModel.DataAnnotations;

namespace UserService.Modules.Roles.Dto;

public class RoleRequest
{
    public string? Id { get; set; }

    [Required]
    public required string Name { get; set; }
}
