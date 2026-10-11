using UserService.Modules.Menus.Model;

namespace UserService.Modules.Menus.Dto;

public class MenuResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string? ParentId { get; set; }

    public List<MenuResponse> Children { get; set; } = [];
}
