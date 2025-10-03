using Microsoft.AspNetCore.Identity;

namespace UserService.Modules.Users
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
