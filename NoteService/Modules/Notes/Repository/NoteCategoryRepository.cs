using Common.EFCore;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using System.Linq.Expressions;

namespace NoteService.Modules.Notes.Repository
{
    public class NoteCategoryRepository(NoteDbContext dbContext)
        : RepositoryBase<NoteCategory, NoteCategoryResponse, NoteDbContext>(dbContext)
    {
        protected override HashSet<string> AllowedFields { get; } = ["Name"];

        protected override Expression<Func<NoteCategoryResponse, object>> DefaultOrderBy
            => x => x.Name;

        protected override IQueryable<NoteCategoryResponse> BuildResponseQuery()
        {
            return
                from n in DbContext.NoteCategories
                select new NoteCategoryResponse
                {
                    Id = n.Id,
                    Name = n.Name
                };
        }
    }
}
