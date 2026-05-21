using Azure.Core;
using Common.Pagination;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace NoteService.Modules.Notes.Repository
{
    public class NoteRepository(NoteDbContext dbContext) : IRepository<Note, NoteResponse>
    {
        private IQueryable<NoteResponse> BuildNoteQuery()
        {
            return
                from n in dbContext.Notes
                join c in dbContext.NoteCategories on n.CategoryId equals c.Id into cat
                from c in cat.DefaultIfEmpty()
                join u in dbContext.Users on n.CreaterId equals u.OldUserId
                select new NoteResponse
                {
                    Id = n.Id,
                    Name = n.Name,
                    CategoryName = c != null ? c.Name : null,
                    CategoryId = n.CategoryId,
                    Content = n.Content,
                    CreaterName = u.Name ?? u.UserName,
                    CreatedAt = n.CreatedAt
                };
        }
        public async Task<Note?> GetByIdAsync(string id)
        {
            return await dbContext.Notes
        .FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<NoteResponse?> GetResponseByIdAsync(string id)
        {
            return await BuildNoteQuery()
        .FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<PagedResult<NoteResponse>> GetListAsync(PagedRequest request)
        {
            int skip = (request.Page - 1) * request.PageSize;

            var query = BuildNoteQuery();

            // 1️⃣ 搜索过滤
            if (!string.IsNullOrEmpty(request.Search))
            {
                string lowerSearch = request.Search.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(lowerSearch)
                    || (u.Name != null && u.Name.ToLower().Contains(lowerSearch))
                );
            }

            // 2️⃣ 排序
            if (!string.IsNullOrEmpty(request.SortField))
            {
                var prop = typeof(NoteResponse).GetProperty(
                    request.SortField,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );
                if (prop != null)
                {
                    query = request.SortDesc
                        ? query.OrderByDescending(u => EF.Property<object>(u, prop.Name))
                        : query.OrderBy(u => EF.Property<object>(u, prop.Name));
                }
            }
            else
            {
                // 默认排序
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            var total = query.Count();
            var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            return new PagedResult<NoteResponse>
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = items,
            };
        }
        public void Create(Note entity)
        {
            dbContext.Notes.Add(entity);
        }
        public void Update(Note entity)
        {
            dbContext.Notes.Update(entity);
        }
        public void Remove(Note entity)
        {
            dbContext.Notes.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            try { 
                var s = await dbContext.SaveChangesAsync();
                return s;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving changes: " + ex.Message);
                throw; // 重新抛出异常以便上层处理
            }
            return await dbContext.SaveChangesAsync();
        }

    }
}
