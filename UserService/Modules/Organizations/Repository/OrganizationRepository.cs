using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Organizations;

namespace UserService.Modules.Organizations.Repository
{
    public class OrganizationRepository(UserDbContext dbContext) : IOrganizationRepository
    {
        public async Task<Organization?> GetAsync(string id)
        {
            return await dbContext.Organizations.FindAsync(id);
        }

        public async Task<List<Organization>> GetListAsync()
        {
            return await dbContext.Organizations.ToListAsync();
        }

        public void Create(Organization organization)
        {
            dbContext.Organizations.Add(organization);
        }

        public void Update(Organization organization)
        {
            dbContext.Organizations.Update(organization);
        }

        public void Delete(Organization organization)
        {
            dbContext.Organizations.Remove(organization);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
