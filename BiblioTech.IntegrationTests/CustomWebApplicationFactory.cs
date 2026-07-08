using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using BiblioTech.Infrastructure.Persistence;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using BiblioTech.API;

namespace BiblioTech.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("bibliotech_test_db")
            .WithUsername("postgres")
            .WithPassword("password123")
            .Build();

        public async Task InitializeAsync() => await _dbContainer.StartAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BiblioTechDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<BiblioTechDbContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BiblioTechDbContext>();
                
                db.Database.OpenConnection();
                using (var command = db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";";
                    command.ExecuteNonQuery();
                }

                db.Database.EnsureCreated();
            }
            return host;
        }

        public new async Task DisposeAsync() => await _dbContainer.StopAsync();
    }
}