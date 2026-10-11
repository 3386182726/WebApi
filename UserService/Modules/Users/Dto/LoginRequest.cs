using System.Text.Json.Serialization;

namespace UserService.Modules.Users.Dto;

public class LoginRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}
