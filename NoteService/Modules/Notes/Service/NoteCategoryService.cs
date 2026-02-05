using Common.Pagination;
using Common.Repository;
using Common.Service;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Service
{
    public class NoteCategoryService(IRepository<NoteCategory, NoteCategoryRequest> repository) : IService<NoteCategory, NoteCategoryRequest>
    {


        public async Task<NoteCategory?> GetByIdAsync(string id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<PagedResult<NoteCategoryRequest>> GetListAsync(PagedRequest pagedRequest)
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
    }
}
