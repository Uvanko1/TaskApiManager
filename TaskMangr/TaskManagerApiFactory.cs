using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskMgr.Data;

namespace TaskMangr;

public sealed class TaskManagerApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public TaskManagerApiFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TaskDbContext>>();
            services.RemoveAll<TaskDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<TaskDbContext>>();

            services.AddDbContext<TaskDbContext>(
                options => options.UseSqlite(_connection));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<TaskDbContext>();

        await context.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}