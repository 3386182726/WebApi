namespace UserService.Modules.Menus.Model
{
    public class RoleMenu
    {
        public string Id { get; set; } = null!;
        public required string RoleId { get; set; }
        public required string MenuId { get; set; }
    }
}
