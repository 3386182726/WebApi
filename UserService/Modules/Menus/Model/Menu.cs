namespace UserService.Modules.Menus.Model
{
    public class Menu
    {
        public string Id { get; set; } = null!;
        public required string Name { get; set; }
        public required string Path { get; set; }
        public string? ParentId { get; set; }
    }
}
