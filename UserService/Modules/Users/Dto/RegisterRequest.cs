using System.ComponentModel.DataAnnotations;

namespace UserService.Modules.Users.Dto;

public class RegisterRequest
{
    public required string UserName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}
