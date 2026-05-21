using NoteService.Modules.Notes.Model;
namespace NoteService.Modules.Notes.Dto
{
    public class NoteResponse
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Content { get; set; }

        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreaterName { get; set; }

        public static NoteResponse FromNote(Note note, string createrName)
        {
            return new NoteResponse
            {
                Id = note.Id,
                Name = note.Name,
                Content = note.Content,
                CategoryId =note.CategoryId,
                CategoryName = note.Category?.Name,
                CreatedAt = note.CreatedAt,
                CreaterName = createrName
            };
        }
    }
}
