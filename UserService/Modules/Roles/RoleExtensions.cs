using UserService.Modules.Roles.Repository;
using UserService.Modules.Roles.Service;

namespace UserService.Modules.Roles
{
    public static class RoleExtensions
    {
        public static IServiceCollection AddRoleService(this IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            return services;
        }
    }
}
