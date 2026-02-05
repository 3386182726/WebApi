using System.Text.Json.Serialization;

namespace NoteService.Modules.Notes.Model
{
    public class Note
    {
        public string Id { get; set; } = null!;
        public required string Name { get; set; }
        public required string Content { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NoteCategory? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreaterId { get; set; }
    }
}
