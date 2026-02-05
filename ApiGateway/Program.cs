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
            // 注册 Ocelot
            builder.Services.AddOcelot();

            // 添加 Cookie 认证
            //builder
            //    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = ".AspNetCore.Cookies"; // 与浏览器一致
            //        options.LoginPath = "/api/user/login"; // 未认证重定向路径（可选）
            //        options.LogoutPath = "/api/user/logout"; // 登出路径
            //        options.Cookie.HttpOnly = true; // 防 JS 获取
            //        options.Cookie.SameSite = SameSiteMode.None;
            //        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 生产环境启用 HTTPS
            //    });
            // 配置 JWT 验证参数
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

                    // 当 Token 通过验证后执行此事件
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userId = context.Principal?.FindFirst("sub")?.Value
                                         ?? context.Principal?.FindFirst("UserId")?.Value;

                            if (!string.IsNullOrEmpty(userId))
                            {
                                // 注入到 HttpContext，以便中间件读取
                                context.HttpContext.Items["UserId"] = userId;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:5173") // 你的前端地址
                            .AllowCredentials() // 允许带上 Cookie
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

            var app = builder.Build();

            app.MapGet("/", () => "Hello ApiGateway ocelot!");

            // 使用 CORS
            app.UseCors("AllowFrontend");

            app.UseAuthentication(); // 必须在 UseAuthorization 前
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                // 处理预检请求（CORS OPTIONS）
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.CompleteAsync();
                    return;
                }
                // 把 Token 中解析出的 UserId 注入下游 Header
                if (context.Items.ContainsKey("UserId"))
                {
                    var userId = context.Items["UserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        context.Request.Headers["X-UserId"] = userId;
                    }
                }
                // 继续执行管道
                await next();
            });

            // 使用 Ocelot
            await app.UseOcelot();

            app.Run();
        }
    }
}
