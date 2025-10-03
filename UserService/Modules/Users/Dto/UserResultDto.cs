namespace UserService.Modules.Users.Dto
{
    public class UserResultDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
        public List<string?> Roles { get; set; } = [];
    }
}
