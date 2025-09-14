using Microsoft.AspNetCore.Cors.Infrastructure;
using WebApi.Modules.User.Repository;
using WebApi.Modules.User.Service;

namespace WebApi.Modules.User
{
    public static class UserExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
