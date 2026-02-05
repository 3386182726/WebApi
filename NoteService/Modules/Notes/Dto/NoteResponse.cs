using NoteService.Modules.Notes.Model;
namespace NoteService.Modules.Notes.Dto
{
    public class NoteResponse
    {
        public required string Name { get; set; }
        public string? Content { get; set; }
        public NoteCategory? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreaterName { get; set; }
    }
}
