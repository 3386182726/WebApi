using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NoteService.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace NoteService.Test
{
    public class CustomWebApplicationFactory
      : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private  MsSqlContainer _container= default!;
        public string ConnectionString => _container.GetConnectionString();
        // 1️⃣ 初始化容器
        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder()
                .WithPassword("Test1234!")
                .Build();

            await _container.StartAsync();

            // 2️⃣ 等容器启动后再跑迁移
            var options = new DbContextOptionsBuilder<NoteDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using var context = new NoteDbContext(options);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseWebRoot("wwwroot");
            builder.ConfigureServices(services =>
            {
                // 移除原来的 DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<NoteDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // 注入 Testcontainer 连接
                services.AddDbContext<NoteDbContext>(options =>
                    options.UseSqlServer(_container.GetConnectionString()));
            });
        }
    }

}