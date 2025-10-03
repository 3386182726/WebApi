using UserService.Modules.Users.Repository;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Users
{
    public static class UserExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, Service.UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
