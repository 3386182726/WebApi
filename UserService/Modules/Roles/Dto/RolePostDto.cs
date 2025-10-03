using System.ComponentModel.DataAnnotations;
using UserService.Modules.Menus.Dto;

namespace UserService.Modules.Roles.Dto
{
    public class RolePostDto
    {
        public string? Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
