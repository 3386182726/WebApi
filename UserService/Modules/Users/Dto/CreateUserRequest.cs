using System.ComponentModel.DataAnnotations;

namespace UserService.Modules.Users.Dto;

public class CreateUserRequest
{
    public string? Id { get; set; }
    public required string UserName { get; set; }

    public string? Name { get; set; }

    public string? Introduction { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public string? OrganizationId { get; set; }
    public List<string> Roles { get; set; } = [];
}
