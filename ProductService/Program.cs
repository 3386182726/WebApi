using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductService.Data;
using ProductService.Modules.Products;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddProductService();
            builder.WebHost.UseUrls("http://0.0.0.0:8080");
            builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            var app = builder.Build();
            app.MapGet("/", () => "Hello Product!");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseStaticFiles(); // 允许访问 wwwroot 下的文件

            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health");
            app.Run();
        }
    }
}
