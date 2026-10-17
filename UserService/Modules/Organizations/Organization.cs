namespace UserService.Modules.Organizations
{
    public class Organization
    {
        public string Id { get; set; } = null!;
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }
    }
}
