using Microsoft.EntityFrameworkCore;
using TaskMgr.Models;

namespace TaskMgr.Data;

public class TaskDbContext: DbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public TaskDbContext(DbContextOptions<TaskDbContext> options): base(options)
    {
        
    }
}
