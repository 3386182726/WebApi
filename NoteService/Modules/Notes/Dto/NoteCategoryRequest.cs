namespace NoteService.Modules.Notes.Dto
{
    public class NoteCategoryRequest
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
    }
}
