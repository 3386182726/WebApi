using Common.Pagination;
using Common.Repository;
using Common.Service;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using NoteService.Modules.Notes.Repository;

namespace NoteService.Modules.Notes.Service
{
    public class NoteCategoryService(INoteCategoryRepository repository) : INoteCategoryService
    {

        public async Task<NoteCategory?> GetByIdAsync(string id)
        {
            return await repository.GetByIdAsync(id);
        }
        public async Task<NoteCategoryResponse?> GetResponseByIdAsync(string id)
        {
            return await repository.GetResponseByIdAsync(id);
        }

        public async Task<PagedResult<NoteCategoryResponse>> GetListAsync(PagedRequest pagedRequest)
        {
            return await repository.GetListAsync(pagedRequest);
        }
        public void Create(NoteCategory entity)
        {
            repository.Create(entity);
        }
        public void Update(NoteCategory entity)
        {
            repository.Update(entity);
        }
        public void Remove(NoteCategory entity)
        {
            repository.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<NoteCategoryResponse>> GetNoteCategoriesAsync()
        {
            var noteCategories = await repository.GetNoteCategoriesAsync();
            return noteCategories.Select(NoteCategoryResponse.FromModel);
        }
    }
}
