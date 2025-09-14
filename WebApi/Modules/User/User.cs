using Microsoft.AspNetCore.Identity;

namespace WebApi.Modules.User
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
