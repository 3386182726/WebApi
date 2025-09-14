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

            // ��� Cookie ��֤
            builder
                .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".AspNetCore.Cookies"; // �������һ��
                    options.LoginPath = "/api/user/login"; // δ��֤�ض���·������ѡ��
                    options.LogoutPath = "/api/user/logout"; // �ǳ�·��
                    options.Cookie.HttpOnly = true; // �� JS ��ȡ
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ������������ HTTPS
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins("https://localhost:5173") // ���ǰ�˵�ַ
                            .AllowCredentials() // ������� Cookie
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
            // ʹ�� CORS
            app.UseCors("AllowFrontend");
            app.UseAuthentication(); // ������ UseAuthorization ǰ
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
