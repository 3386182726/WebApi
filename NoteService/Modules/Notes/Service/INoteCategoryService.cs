using Common.Service;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Service
{
    public interface INoteCategoryService : IService<NoteCategory, NoteCategoryResponse>
    {
        Task<IEnumerable<NoteCategoryResponse>> GetNoteCategoriesAsync();
    }
}
