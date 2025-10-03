namespace UserService.Modules.Menus.Dto
{
    public class MenuPostDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Path { get; set; }
        public string? ParentId { get; set; }
    }
}
