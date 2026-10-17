using Microsoft.EntityFrameworkCore;
using FileService.Data;

namespace FileService.Modules.Files
{
    public interface IAppFileRepository
    {
        Task<List<AppFile>> GetByOwnerAsync(string ownerType, string ownerId, string? category = null, string? type = null);
        Task<List<AppFile>> GetByOwnersAsync(string ownerType, IEnumerable<string> ownerIds, string? category = null);
        Task<AppFile?> GetByIdAsync(string id);
        void Create(AppFile file);
        void Remove(AppFile file);
        Task<int> SaveChangesAsync();
    }

    public class AppFileRepository(FileDbContext dbContext) : IAppFileRepository
    {
        public async Task<List<AppFile>> GetByOwnerAsync(string ownerType, string ownerId, string? category = null, string? type = null)
        {
            IQueryable<AppFile> query = dbContext.AppFiles
                .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category == category);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(x => x.Type == type);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<List<AppFile>> GetByOwnersAsync(string ownerType, IEnumerable<string> ownerIds, string? category = null)
        {
            var ids = ownerIds.ToList();
            if (ids.Count == 0) return [];

            IQueryable<AppFile> query = dbContext.AppFiles
                .Where(x => x.OwnerType == ownerType && ids.Contains(x.OwnerId));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category == category);

            return await query.ToListAsync();
        }

        public async Task<AppFile?> GetByIdAsync(string id)
            => await dbContext.AppFiles.FindAsync(id);

        public void Create(AppFile file)
            => dbContext.AppFiles.Add(file);

        public void Remove(AppFile file)
            => dbContext.AppFiles.Remove(file);

        public async Task<int> SaveChangesAsync()
            => await dbContext.SaveChangesAsync();
    }
}
