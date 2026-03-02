using NoteService.Modules.Notes.Model;
namespace NoteService.Modules.Notes.Dto
{
    public class NoteRequest
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Content { get; set; }
        public NoteCategory? Category { get; set; }
    }
}
