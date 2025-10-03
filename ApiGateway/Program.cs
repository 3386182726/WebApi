using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            // ע�� Ocelot
            builder.Services.AddOcelot();

            // ��� Cookie ��֤
            //builder
            //    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = ".AspNetCore.Cookies"; // �������һ��
            //        options.LoginPath = "/api/user/login"; // δ��֤�ض���·������ѡ��
            //        options.LogoutPath = "/api/user/logout"; // �ǳ�·��
            //        options.Cookie.HttpOnly = true; // �� JS ��ȡ
            //        options.Cookie.SameSite = SameSiteMode.None;
            //        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ������������ HTTPS
            //    });
            // ���� JWT ��֤����
            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        ),
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:5173") // ���ǰ�˵�ַ
                            .AllowCredentials() // ������� Cookie
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

            var app = builder.Build();

            app.MapGet("/", () => "Hello ApiGateway ocelot!");

            // ʹ�� CORS
            app.UseCors("AllowFrontend");
            app.Use(
                async (context, next) =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.CompleteAsync();
                    }
                    else
                    {
                        await next();
                    }
                }
            );
            app.UseAuthentication(); // ������ UseAuthorization ǰ
            app.UseAuthorization();

            // ʹ�� Ocelot
            await app.UseOcelot();

            app.Run();
        }
    }
}
