using Microsoft.EntityFrameworkCore;
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

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddProductService();

            var app = builder.Build();
            app.MapGet("/", () => "Hello Product!");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseStaticFiles(); // 允许访问 wwwroot 下的文件

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
