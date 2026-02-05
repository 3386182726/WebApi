using Common.Pagination;
using Common.Repository;
using Common.Service;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Service
{
    public class NoteService(IRepository<Note, NoteResponse> repository) : IService<Note, NoteResponse>
    {

        public async Task<Note?> GetByIdAsync(string id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<PagedResult<NoteResponse>> GetListAsync(PagedRequest pagedRequest)
        {
             return await repository.GetListAsync(pagedRequest);
        }
        public void Create(Note entity)
        {
             repository.Create(entity);
        }
        public void Update(Note entity)
        {
            repository.Update(entity);
        }
        public void Remove(Note entity)
        {
            repository.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await repository.SaveChangesAsync();
        }
    }
}
