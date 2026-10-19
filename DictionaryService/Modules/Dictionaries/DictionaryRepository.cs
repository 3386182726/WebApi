using Common.Abstractions;
using Common.EFCore;
using DictionaryService.Data;
using Microsoft.EntityFrameworkCore;

namespace DictionaryService.Modules.Dictionaries
{
    /// <summary>
    /// 纯数据库访问：只负责 Dictionary 表的 EF 查询。
    /// </summary>
    public interface IDictionaryRepository : IRepository<Dictionary, DictionaryResponse>
    {
        Task<List<DictionaryResponse>> GetByTypeAsync(string dictType, bool? isActive = null);
        Task<List<DictionaryResponse>> GetByTypesAsync(IEnumerable<string> dictTypes, bool? isActive = null);
        Task<DictionaryResponse?> GetByIdResponseAsync(string id);
        Task<Common.Pagination.PagedResult<DictionaryResponse>> GetPagedAsync(Common.Pagination.PagedRequest request);
    }

    public class DictionaryRepository(DictionaryDbContext dbContext)
        : RepositoryBase<Dictionary, DictionaryResponse, DictionaryDbContext>(dbContext), IDictionaryRepository
    {
        protected override HashSet<string> AllowedFields { get; } = ["DictType", "Code", "Name", "Sort", "IsActive"];

        protected override IQueryable<DictionaryResponse> BuildResponseQuery()
            => Set.Select(d => new DictionaryResponse
            {
                Id = d.Id,
                DictType = d.DictType,
                Code = d.Code,
                Name = d.Name,
                ParentId = d.ParentId,
                Sort = d.Sort,
                IsActive = d.IsActive,
                Description = d.Description,
            });

        public async Task<DictionaryResponse?> GetByIdResponseAsync(string id)
            => await base.GetResponseByIdAsync(id);

        public async Task<Common.Pagination.PagedResult<DictionaryResponse>> GetPagedAsync(Common.Pagination.PagedRequest request)
            => await GetListAsync(request);

        public async Task<List<DictionaryResponse>> GetByTypeAsync(string dictType, bool? isActive = null)
        {
            var query = BuildResponseQuery().Where(d => d.DictType == dictType);
            if (isActive.HasValue) query = query.Where(d => d.IsActive == isActive.Value);
            return await query.OrderBy(d => d.Sort).ThenBy(d => d.Code).ToListAsync();
        }

        public async Task<List<DictionaryResponse>> GetByTypesAsync(IEnumerable<string> dictTypes, bool? isActive = null)
        {
            var types = dictTypes.ToList();
            if (types.Count == 0) return [];
            var query = BuildResponseQuery().Where(d => types.Contains(d.DictType));
            if (isActive.HasValue) query = query.Where(d => d.IsActive == isActive.Value);
            return await query.OrderBy(d => d.DictType).ThenBy(d => d.Sort).ThenBy(d => d.Code).ToListAsync();
        }
    }
}
