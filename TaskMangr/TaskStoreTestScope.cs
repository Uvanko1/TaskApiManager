using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskMgr.Data;
using TaskMgr.Services;

namespace TaskMangr;

public sealed class TaskStoreTestScope : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TaskDbContext> _options;

    public TaskDbContext Context { get; }
    public TaskStore Store { get; }

    private TaskStoreTestScope(
        SqliteConnection connection,
        DbContextOptions<TaskDbContext> options,
        TaskDbContext context)
    {
        _connection = connection;
        _options = options;
        Context = context;
        Store = new TaskStore(context);
    }

    public static async Task<TaskStoreTestScope> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TaskDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return new TaskStoreTestScope(connection, options, context);
    }

    public TaskDbContext CreateContext()
    {
        return new TaskDbContext(_options);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}