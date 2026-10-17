using UserService.Modules.Organizations.Repository;
using UserService.Modules.Organizations.Service;

namespace UserService.Modules.Organizations
{
    public static class OrganizationExtensions
    {
        public static IServiceCollection AddOrganizationService(this IServiceCollection services)
        {
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            return services;
        }
    }
}
