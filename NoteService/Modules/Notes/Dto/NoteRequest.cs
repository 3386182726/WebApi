using NoteService.Modules.Notes.Model;
namespace NoteService.Modules.Notes.Dto
{
    public class NoteRequest
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }
        public required NoteCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreaterId { get; set; }
    }
}
