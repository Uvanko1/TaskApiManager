namespace TaskMangr;
using TaskMgr.Services;
using TaskMgr.Data;
using TaskMgr.Models;
using System.Reflection;

public class TaskStoreTests
{
    [Fact]
    public async Task Add_ThenGetById_ReturnsSavedTask()
    {
        await using var scope = await TaskStoreTestScope.CreateAsync();

        var task = new TaskItem
        {
            Title = "Test task",
            IsCompleted = false
        };

        await scope.Store.Add(task);

        await using var newContext = scope.CreateContext();
        var newStore = new TaskStore(newContext);

        var findTask = await newStore.GetById(task.Id);
        Assert.NotNull(findTask);
        Assert.Equal(task.Id, findTask.Id);
        Assert.Equal(task.Title, findTask.Title);
        Assert.False(findTask.IsCompleted);

    }

    [Fact]
    public async Task GetById_WhenTaskDoesNotExist_ReturnsNull()
    {
        await using var scope = await TaskStoreTestScope.CreateAsync();

        var task = await scope.Store.GetById(999);

        Assert.Null(task);
    }

    [Fact]
    public async Task UpdateById_WhenTaskExists_UpdatesTask()
    {
        await using var scope = await TaskStoreTestScope.CreateAsync();

        var task = new TaskItem
        {
            Title = "OldTask",
            IsCompleted = false
        };

        await scope.Store.Add(task);

        var updateResult = await scope.Store.UpdateById(new TaskItem { Title = "NewTask", IsCompleted = true }, task.Id);

        await using var newContext = scope.CreateContext();

        var newStore = new TaskStore(newContext);

        var updatedTask = await newStore.GetById(task.Id);

        Assert.True(updateResult);
        Assert.NotNull(updatedTask);
        Assert.Equal(task.Id, updatedTask.Id);
        Assert.Equal("NewTask", updatedTask.Title);
        Assert.True(updatedTask.IsCompleted);
    }

    [Fact]
    public async Task DeleteById_WhenTaskExists_DeletesTask()
    {
        await using var scope = await TaskStoreTestScope.CreateAsync();

        var task = new TaskItem
        {
            Title = "Task to delete",
            IsCompleted = true
        };


        await scope.Store.Add(task);

        var deleteResult = await scope.Store.DeleteById(task.Id);

        await using var newContext = scope.CreateContext();

        var newStore = new TaskStore(newContext);

        var deletedTask = await newStore.GetById(task.Id);
        Assert.True(deleteResult);
        Assert.Null(deletedTask);
    }
}
