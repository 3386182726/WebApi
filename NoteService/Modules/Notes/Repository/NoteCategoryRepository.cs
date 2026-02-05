using Common.Pagination;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using System.Reflection;

namespace NoteService.Modules.Notes.Repository
{
    public class NoteCategoryRepository(NoteDbContext noteDbContext) : IRepository<NoteCategory, NoteCategoryRequest>
    {
        public async Task<NoteCategory?> GetByIdAsync(string id)
        {
            return await noteDbContext.NoteCategories
            .Where(n => n.Id == id)
            .FirstOrDefaultAsync();
        }
        public async Task<PagedResult<NoteCategoryRequest>> GetListAsync(PagedRequest request)
        {
            int skip = (request.Page - 1) * request.PageSize;

            var query =
                from n in noteDbContext.NoteCategories
                select new NoteCategoryRequest
                {
                    Name = n.Name
                };


            // 1️⃣ 搜索过滤
            if (!string.IsNullOrEmpty(request.Search))
            {
                string lowerSearch = request.Search.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(lowerSearch)
                    || (u.Name != null && u.Name.ToLower().Contains(lowerSearch))
                );
            }
                // 默认排序
            query = query.OrderBy(u => u.Name);

            var total = query.Count();
            var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            return new PagedResult<NoteCategoryRequest>
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = items,
            };
        }
        public void Create(NoteCategory entity)
        {
            noteDbContext.NoteCategories.Add(entity);
        }

        public void Update(NoteCategory entity)
        {
            noteDbContext.NoteCategories.Update(entity);
        }
        public void Remove(NoteCategory entity)
        {
            noteDbContext.NoteCategories.Remove(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
           return await noteDbContext.SaveChangesAsync();
        }
    }
}
