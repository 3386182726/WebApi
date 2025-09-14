using System.ComponentModel.DataAnnotations;

namespace WebApi.Modules.Role.Dto
{
    public class RoleDto
    {
        public string? Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
