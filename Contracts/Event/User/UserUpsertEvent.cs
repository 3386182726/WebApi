namespace Contracts.Event.User
{
    public class UserUpsertEvent
    {
        public required string UserId { get; set; } 
        public required string UserName { get; set; }
        public string? Name { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
