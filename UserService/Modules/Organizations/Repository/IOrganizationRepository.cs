using UserService.Modules.Organizations;

namespace UserService.Modules.Organizations.Repository
{
    public interface IOrganizationRepository
    {
        Task<Organization?> GetAsync(string id);
        Task<List<Organization>> GetListAsync();
        void Create(Organization organization);
        void Update(Organization organization);
        void Delete(Organization organization);
        Task<int> SaveChangesAsync();
    }
}
