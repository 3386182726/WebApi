namespace NoteService.Modules.Notes.Model
{
    public class User
    {
        public string Id { get; set; } = null!;
        public required string OldUserId { get; set; }
        public required string UserName { get; set; }
        public required string Name { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
