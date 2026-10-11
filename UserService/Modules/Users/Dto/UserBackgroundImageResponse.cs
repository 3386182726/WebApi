namespace UserService.Modules.Users.Dto;

public class UserBackgroundImageResponse
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; }
}
