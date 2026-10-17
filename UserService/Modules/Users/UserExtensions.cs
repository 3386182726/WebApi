using Common.Abstractions;
using Common.Service;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Repository;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Users
{
    public static class UserExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, Service.UserService>();
            services.AddScoped<IRepository<User, UserResponse>, UserRepository>();
            services.AddScoped<IUploadService, UploadService>();
            return services;
        }
    }
}
