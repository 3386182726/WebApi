using System.ComponentModel.DataAnnotations;

namespace UserService.Modules.Users.Dto
{
    public class CreateDto
    {
        public string? Id { get; set; }
        public required string UserName { get; set; }

        public string? Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
