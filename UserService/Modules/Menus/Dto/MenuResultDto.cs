using UserService.Modules.Menus.Model;

namespace UserService.Modules.Menus.Dto
{
    public class MenuResultDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Path { get; set; }
        public string? ParentId { get; set; }

        public List<MenuResultDto> Children { get; set; } = [];
    }
}
