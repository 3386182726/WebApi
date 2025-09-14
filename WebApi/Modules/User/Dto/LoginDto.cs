using System.Text.Json.Serialization;

namespace WebApi.Modules.User.Dto
{
    public class LoginDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}
