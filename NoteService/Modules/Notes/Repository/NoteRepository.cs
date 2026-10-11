using Common.EFCore;
using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using System.Linq.Expressions;

namespace NoteService.Modules.Notes.Repository
{
    public class NoteRepository(NoteDbContext dbContext)
        : RepositoryBase<Note, NoteResponse, NoteDbContext>(dbContext)
    {
        protected override HashSet<string> AllowedFields { get; } = ["Name", "Content","CategoryId"];

        protected override Expression<Func<NoteResponse, object>> DefaultOrderBy
            => x => EF.Property<object>(x, "CreatedAt");

        protected override IQueryable<NoteResponse> BuildResponseQuery()
        {
            return
                from n in DbContext.Notes
                join c in DbContext.NoteCategories on n.CategoryId equals c.Id into cat
                from c in cat.DefaultIfEmpty()
                join u in DbContext.Users on n.CreaterId equals u.OldUserId
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
    }
}
