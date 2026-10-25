namespace TaskMgr.Services;
using TaskMgr.Models;
using TaskMgr.Data;
using Microsoft.EntityFrameworkCore;

public class TaskStore
{
    private readonly TaskDbContext taskDbContext;

    public TaskStore(TaskDbContext context)
    {
        taskDbContext = context;
    }

    public async Task<TaskItem[]> GetAll()
    {
        var allTasks = await taskDbContext.Tasks.ToArrayAsync();
        return allTasks;
    }

    public async Task Add(TaskItem task)
    {
        taskDbContext.Tasks.Add(task);
        await taskDbContext.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetById(int id)
    {
        var task = await taskDbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        return task;
    }

    public async Task<bool> DeleteById(int id)
    {
        var task = await taskDbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (task == null)
        {
            return false;
        }
        taskDbContext.Tasks.Remove(task);
        await taskDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateById(TaskItem newTask, int id)
    {
        var task = await taskDbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (task == null)
        {
            return false;
        }
        task.Title = newTask.Title;
        task.IsCompleted = newTask.IsCompleted;
        await taskDbContext.SaveChangesAsync();
        return true;
    }
}
