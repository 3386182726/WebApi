using Microsoft.AspNetCore.Identity;

namespace UserService.Modules.Users
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Introduction { get; set; }
        public DateTime? Birthday { get; set; }
        public string? OrganizationId { get; set; }
    }
}
