using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace NoteService.Test.Utils
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public MsSqlContainer Container { get; private set; } = default!;
        public string ConnectionString => Container.GetConnectionString();
        public async Task InitializeAsync()
        {
            Container = new MsSqlBuilder()
                .WithPassword("Test1234!")
                .Build();

            await Container.StartAsync();

            // 自动跑 Migration
            var options = new DbContextOptionsBuilder<NoteDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using var context = new NoteDbContext(options);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }
    }
}
