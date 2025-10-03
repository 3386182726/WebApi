using UserService.Modules.Menus.Repository;

namespace UserService.Modules.Users
{
    public static class MenuExtensions
    {
        public static IServiceCollection AddMenuService(this IServiceCollection services)
        {
            services.AddScoped<IMenuRepository, MenuRepository>();
            return services;
        }
    }
}
