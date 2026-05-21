using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Dto
{
    public class NoteCategoryResponse
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        public static NoteCategoryResponse FromModel(NoteCategory model)
        {
            return new NoteCategoryResponse
            {
                Id = model.Id,
                Name = model.Name
            };
        }
    }
}
