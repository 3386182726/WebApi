using Common.Repository;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Repository
{
    public interface INoteCategoryRepository : IRepository<NoteCategory, NoteCategoryResponse>
    {
        Task<IEnumerable<NoteCategory>> GetNoteCategoriesAsync();
    }
}
