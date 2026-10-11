namespace UserService.Modules.Users.Model
{
    /// <summary>
    /// 用户上传的背景图
    /// </summary>
    public class UserBackgroundImage
    {
        public string Id { get; set; } = null!;
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
