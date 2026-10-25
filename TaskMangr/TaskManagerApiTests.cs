namespace TaskMangr;
using System.Net;
using System.Net.Http.Json;
using TaskMgr.Models;
using TaskMgr.DTOs;

public class TaskManagerApiTests
{
    [Fact]
    public async Task GetTasks_ReturnsOkAndEmptyArray()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonResponse = await response.Content.ReadFromJsonAsync<TaskItem[]>();
        Assert.NotNull(jsonResponse);
        Assert.Empty(jsonResponse);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAndCanBeRetrieved()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var taskDto = new CreateTaskDto
        {
            Title = "Integration task",
            IsCompleted = false
        };
        using var response = await client.PostAsJsonAsync("/api/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonResponse = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(jsonResponse);
        Assert.True(jsonResponse.Id > 0);
        Assert.False(jsonResponse.IsCompleted);

        using var getResponse = await client.GetAsync($"/api/tasks/{jsonResponse.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var jsonGetResponse = await getResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(jsonGetResponse);
        Assert.Equal(jsonResponse.Id, jsonGetResponse.Id);
    }

    [Fact]
    public async Task CreateTask_WithEmptyTitle_ReturnsBadRequestAndDoesNotSaveTask()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var taskDto = new CreateTaskDto
        {
            Title = "",
            IsCompleted = false
        };
        using var response = await client.PostAsJsonAsync("/api/tasks", taskDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var getResponse = await client.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResponseJson = await getResponse.Content.ReadFromJsonAsync<TaskItem[]>();
        Assert.NotNull(getResponseJson);
        Assert.Empty(getResponseJson);
    }

    [Fact]
    public async Task NotFoundTask_ReturnsNotFoundCode()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/tasks/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_UpdatesExistingTask()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        var taskDto = new CreateTaskDto
        {
            Title = "Old task",
            IsCompleted = false
        };

        using var response = await client.PostAsJsonAsync("/api/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);

        var newTaskDto = new CreateTaskDto
        {
            Title = "New Task",
            IsCompleted = true
        };

        using var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{task.Id}", newTaskDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        using var getResponse = await client.GetAsync($"/api/tasks/{task.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResponseJson = await getResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(getResponseJson);
        Assert.Equal("New Task", getResponseJson.Title);
        Assert.Equal(task.Id, getResponseJson.Id);
        Assert.True(getResponseJson.IsCompleted);
    }

    [Fact]
    public async Task DeleteTask_DeletesExistingTaskAndNotFoundIt()
    {
        using var factory = new TaskManagerApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        var taskDto = new CreateTaskDto
        {
            Title = "Old task",
            IsCompleted = false
        };

        using var response = await client.PostAsJsonAsync("/api/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);

        using var deleteResponse = await client.DeleteAsync($"/api/tasks/{task.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using var getResponse = await client.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResponseJson = await getResponse.Content.ReadFromJsonAsync<TaskItem[]>();
        Assert.NotNull(getResponseJson);
        Assert.Empty(getResponseJson);
    }
}
