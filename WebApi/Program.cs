using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Modules.User;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder
                .Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // 添加 Cookie 认证
            builder
                .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".AspNetCore.Cookies"; // 与浏览器一致
                    options.LoginPath = "/api/user/login"; // 未认证重定向路径（可选）
                    options.LogoutPath = "/api/user/logout"; // 登出路径
                    options.Cookie.HttpOnly = true; // 防 JS 获取
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 生产环境启用 HTTPS
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins("https://localhost:5173") // 你的前端地址
                            .AllowCredentials() // 允许带上 Cookie
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

            builder.Services.AddUserService();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbInitializer.SeedAsync(services);
            }
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseRouting();
            // 使用 CORS
            app.UseCors("AllowFrontend");
            app.UseAuthentication(); // 必须在 UseAuthorization 前
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
